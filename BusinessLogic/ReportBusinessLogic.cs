using Contracts.DTO;
using Contracts.DTO.Reports;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class ReportBusinessLogic(
    IOperationStorageContract operationStorage, IReportStore store,
    ILogger<ReportBusinessLogic> logger) : IReportBusinessLogic
{
    public ReportResultDto Build(ReportBuildRequestDto request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (request.From == default) throw new ValidationException("From is empty");
        if (request.To == default) throw new ValidationException("To is empty");

        // !!! ВАЖНО: приводим к UTC, иначе Npgsql падает на timestamptz
        request.From = NormalizeUtc(request.From);
        request.To = NormalizeUtc(request.To);

        if (request.From > request.To) throw new ValidationException("From must be <= To");

        var result = request.TypeCode switch
        {
            ReportTypeCodes.ActualCostDistribution => BuildActualCostDistribution(request),
            ReportTypeCodes.SalesStatement => BuildSalesStatement(request),
            ReportTypeCodes.RealisedDeviationStatement => BuildRealisedDeviationStatement(request),
            _ => throw new ValidationException($"Unknown report type: {request.TypeCode}")
        };

        store.Save(result);

        return result;
    }
    public void Delete(string id)
    {
        store.Delete(id);
    }
    public List<ReportListItemDto> GetList(ReportTypeCodes? typeCode = null)
        => store.GetList(typeCode);
    private static DateTime NormalizeUtc(DateTime dt) => dt.Kind switch
    {
        DateTimeKind.Utc => dt,
        DateTimeKind.Local => dt.ToUniversalTime(),
        DateTimeKind.Unspecified => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
        _ => DateTime.SpecifyKind(dt, DateTimeKind.Utc)
    };

    // -------------------- REPORT 1 --------------------
    // Ведомость распределения фактических затрат по видам выпущенной продукции
    private ReportResultDto BuildActualCostDistribution(ReportBuildRequestDto request)
    {
        var acc = operationStorage.GetAccountIdsByNums(new[] { "20", "43" });
        if (!acc.TryGetValue("20", out var acc20) || !acc.TryGetValue("43", out var acc43))
            throw new ValidationException("Accounts 20/43 not found in ChartOfAccount");

        var receipts = operationStorage.GetReceipts43_20_Plan(request.From, request.To, acc43, acc20);
        var deltas = operationStorage.GetAllocDeltas43_20(request.From, request.To, acc43, acc20);

        var productIds = receipts.Keys
            .Union(deltas.Keys)
            .Distinct()
            .ToList();

        var info = operationStorage.GetProductionInfoByIds(productIds);

        var rows = new List<ActualCostDistributionRowDto>();

        foreach (var productId in productIds)
        {
            receipts.TryGetValue(productId, out var r); // (qty,sum)
            deltas.TryGetValue(productId, out var d);   // decimal

            var planSum = r.sum;
            var dev = d;
            var fact = planSum + dev;

            info.TryGetValue(productId, out var pi);

            rows.Add(new ActualCostDistributionRowDto
            {
                ProductionId = productId,
                ProductionCode = pi.code ?? string.Empty,
                ProductionName = pi.name ?? string.Empty,
                Quantity = r.qty,
                PlanCost = planSum,
                Deviation = dev,
                ActualCost = fact
            });
        }

        // Итоги
        var totalQty = rows.Sum(x => x.Quantity);
        var totalPlan = rows.Sum(x => x.PlanCost);
        var totalDev = rows.Sum(x => x.Deviation);
        var totalFact = rows.Sum(x => x.ActualCost);

        // "Общая сумма фактических затрат" (из дебетового оборота 20)
        var totalActualCostsHeader = operationStorage.GetDebitTurnover20(request.From, request.To, acc20);

        return new ReportResultDto
        {
            TypeCode = request.TypeCode,
            From = request.From,
            To = request.To,
            Name = string.IsNullOrWhiteSpace(request.Name)
                ? "Ведомость распределения фактических затрат"
                : request.Name!,
            BuildDate = DateTime.UtcNow,

            TotalActualCosts = totalActualCostsHeader,
            TotalQty = totalQty,
            Total1 = totalPlan, // план
            Total2 = totalDev,  // отклонение
            Total3 = totalFact, // факт

            ActualCostDistributionRows = rows
                .OrderBy(x => x.ProductionCode)
                .ThenBy(x => x.ProductionName)
                .ToList()
        };
    }

    // -------------------- REPORT 2 --------------------
    // Ведомость продаж продукции
    private ReportResultDto BuildSalesStatement(ReportBuildRequestDto request)
    {
        var acc = operationStorage.GetAccountIdsByNums(new[] { "90", "43" });
        if (!acc.TryGetValue("90", out var acc90) || !acc.TryGetValue("43", out var acc43))
            throw new ValidationException("Accounts 90/43 not found in ChartOfAccount");

        // Выручка: берём из документов Sale (у тебя в проводках Дт62 Кт90 без аналитики по продукции)
        var docs = operationStorage.GetAll(request.From, request.To);
        var salesDocs = docs.Where(x => x.Type == OperationType.Sale && !x.IsDeleted).ToList();

        var soldByProduct = salesDocs
            .SelectMany(d => d.Elements ?? new List<ElementDto>())
            .Where(e => !string.IsNullOrWhiteSpace(e.ProductionId))
            .GroupBy(e => e.ProductionId!)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => (decimal)x.CountElement * (x.Price ?? 0m))
            );

        // Себестоимость продаж (план): Дт90 Кт43, count > 0
        var cogs = operationStorage.GetSalesCogs90_43_Plan(request.From, request.To, acc90, acc43);

        var productIds = soldByProduct.Keys
            .Union(cogs.Keys)
            .Distinct()
            .ToList();

        var info = operationStorage.GetProductionInfoByIds(productIds);

        var rows = new List<SalesStatementRowDto>();

        foreach (var productId in productIds)
        {
            soldByProduct.TryGetValue(productId, out var sold);
            cogs.TryGetValue(productId, out var cg); // (qty,sum)

            info.TryGetValue(productId, out var pi);

            var cost = cg.sum;
            var profit = sold - cost;

            rows.Add(new SalesStatementRowDto
            {
                ProductionId = productId,
                ProductionCode = pi.code ?? string.Empty,
                ProductionName = pi.name ?? string.Empty,
                SoldAmount = sold,
                SalesCost = cost,
                ProfitOrLoss = profit
            });
        }

        var totalSold = rows.Sum(x => x.SoldAmount);
        var totalCost = rows.Sum(x => x.SalesCost);
        var totalProfit = rows.Sum(x => x.ProfitOrLoss);

        return new ReportResultDto
        {
            TypeCode = request.TypeCode,
            From = request.From,
            To = request.To,
            Name = string.IsNullOrWhiteSpace(request.Name)
                ? "Ведомость продаж продукции"
                : request.Name!,
            BuildDate = DateTime.UtcNow,

            // Для отчёта 2 qty не обязательно, оставим 0
            TotalQty = 0,
            Total1 = totalSold,
            Total2 = totalCost,
            Total3 = totalProfit,

            SalesStatementRows = rows
                .OrderBy(x => x.ProductionCode)
                .ThenBy(x => x.ProductionName)
                .ToList()
        };
    }

    // -------------------- REPORT 3 --------------------
    // Отклонения фактической себестоимости от плановой по реализованной продукции
    private ReportResultDto BuildRealisedDeviationStatement(ReportBuildRequestDto request)
    {
        var acc = operationStorage.GetAccountIdsByNums(new[] { "90", "43" });
        if (!acc.TryGetValue("90", out var acc90) || !acc.TryGetValue("43", out var acc43))
            throw new ValidationException("Accounts 90/43 not found in ChartOfAccount");

        // Плановая себестоимость продаж: Дт90 Кт43, count > 0
        var plan = operationStorage.GetSalesCogs90_43_Plan(request.From, request.To, acc90, acc43);

        // Отклонение списания: Дт90 Кт43, count == 0
        var dev = operationStorage.GetSalesDeviation90_43(request.From, request.To, acc90, acc43);

        var productIds = plan.Keys
            .Union(dev.Keys)
            .Distinct()
            .ToList();

        var info = operationStorage.GetProductionInfoByIds(productIds);

        var rows = new List<RealisedDeviationRowDto>();

        foreach (var productId in productIds)
        {
            plan.TryGetValue(productId, out var p); // (qty,sum)
            dev.TryGetValue(productId, out var d);

            info.TryGetValue(productId, out var pi);

            var planSum = p.sum;
            var deviation = d;
            var fact = planSum + deviation;

            rows.Add(new RealisedDeviationRowDto
            {
                ProductionId = productId,
                ProductionCode = pi.code ?? string.Empty,
                ProductionName = pi.name ?? string.Empty,
                Quantity = p.qty,
                PlanSalesCost = planSum,
                Deviation = deviation,
                ActualSalesCost = fact
            });
        }

        var totalQty = rows.Sum(x => x.Quantity);
        var totalPlan = rows.Sum(x => x.PlanSalesCost);
        var totalDev = rows.Sum(x => x.Deviation);
        var totalFact = rows.Sum(x => x.ActualSalesCost);

        return new ReportResultDto
        {
            TypeCode = request.TypeCode,
            From = request.From,
            To = request.To,
            Name = string.IsNullOrWhiteSpace(request.Name)
                ? "Ведомость отклонений по реализованной продукции"
                : request.Name!,
            BuildDate = DateTime.UtcNow,

            TotalQty = totalQty,
            Total1 = totalPlan,
            Total2 = totalDev,
            Total3 = totalFact,

            RealisedDeviationRows = rows
                .OrderBy(x => x.ProductionCode)
                .ThenBy(x => x.ProductionName)
                .ToList()
        };
    }
}
