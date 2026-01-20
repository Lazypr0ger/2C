using Contracts.DTO;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class OperationBusinessLogic(
    IOperationStorageContract storage,
    ILogger<OperationBusinessLogic> logger) : IOperationBusinessLogic
{
    public List<OperationDto> GetAll(DateTime? from = null, DateTime? to = null)
    {
        var list = storage.GetAll(from, to);
        return list ?? throw new NullListException();
    }

    // ДОБАВЬ (для журналов документов по типу)
    public List<OperationDto> GetAllByType(OperationType type, DateTime? from = null, DateTime? to = null)
    {
        var list = storage.GetAllByType(type, from, to);
        return list ?? throw new NullListException();
    }

    public OperationDto GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Operation id is empty");

        return storage.GetById(id) ?? throw new ElementNotFoundException(id);
    }

    public void Create(OperationDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        dto.Id ??= Guid.NewGuid().ToString();
        dto.IsDeleted = false;

        NormalizeAndValidate(dto, isUpdate: false);

        // 1) сформировать проводки заранее (чтобы знать сумму)
        var logs = BuildPostings(dto);

        // 2) итог документа: для документов, где сумма должна считаться по проводкам
        dto.TotalAmountDocument = logs.Sum(x => x.Amount);

        // 3) атомарное сохранение шапка+строки+проводки
        storage.CreateWithLinesAndLogs(dto, dto.Elements ?? new(), logs);

        logger.LogInformation("Operation created and posted. Id={Id}, Type={Type}", dto.Id, dto.Type);
    }

    public void Update(OperationDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ValidationException("Operation id is empty");

        dto.IsDeleted = false; // на обновлении документ не должен сам становиться удалённым

        NormalizeAndValidate(dto, isUpdate: true);

        var logs = BuildPostings(dto);
        dto.TotalAmountDocument = logs.Sum(x => x.Amount);

        // атомарно обновить шапку + заменить строки + заменить проводки
        storage.UpdateWithLinesAndLogs(dto, dto.Elements ?? new(), logs);

        logger.LogInformation("Operation updated and reposted. Id={Id}, Type={Type}", dto.Id, dto.Type);
    }

    public void Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Operation id is empty");

        storage.Delete(id);
    }

    public void Recovery(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Operation id is empty");

        storage.Recovery(id);
    }

    private void NormalizeAndValidate(OperationDto dto, bool isUpdate)
    {
        if (string.IsNullOrWhiteSpace(dto.NameDocument))
            throw new ValidationException("NameDocument is empty");

        if (dto.DateOperation == default)
            dto.DateOperation = DateTime.UtcNow;

        // базовая валидация по типам
        switch (dto.Type)
        {
            case OperationType.ActualCosts:
                if (string.IsNullOrWhiteSpace(dto.DepartamentId))
                    throw new ValidationException("DepartamentId is empty for ActualCosts");

                if (dto.TotalAmountDocument <= 0m)
                    throw new ValidationException("TotalAmountDocument must be > 0 for ActualCosts");
                break;

            case OperationType.ReceiptFromProduction:
                if (string.IsNullOrWhiteSpace(dto.DepartamentId))
                    throw new ValidationException("DepartamentId is empty for ReceiptFromProduction");

                if (dto.Elements is null || dto.Elements.Count == 0)
                    throw new ValidationException("Elements is empty for ReceiptFromProduction");

                foreach (var e in dto.Elements)
                {
                    if (string.IsNullOrWhiteSpace(e.ProductionId))
                        throw new ValidationException("Element.ProductionId is empty");
                    if (e.CountElement <= 0)
                        throw new ValidationException("Element.CountElement must be > 0");
                }

                // КЛЮЧЕВОЕ ПРАВИЛО: продукт должен принадлежать выбранному цеху
                ValidateProductionsBelongToDepartament(dto.DepartamentId!, dto.Elements);
                break;

            case OperationType.Sale:
                if (string.IsNullOrWhiteSpace(dto.OrganisationId))
                    throw new ValidationException("OrganisationId is empty for Sale");

                if (dto.Elements is null || dto.Elements.Count == 0)
                    throw new ValidationException("Elements is empty for Sale");

                foreach (var e in dto.Elements)
                {
                    if (string.IsNullOrWhiteSpace(e.ProductionId))
                        throw new ValidationException("Element.ProductionId is empty");
                    if (e.CountElement <= 0)
                        throw new ValidationException("Element.CountElement must be > 0");
                    if (e.Price is null || e.Price <= 0)
                        throw new ValidationException("Element.Price must be > 0 for Sale");
                }
                break;

            case OperationType.AllocateActualCost:
            case OperationType.WriteOffDeviations:
                // эти операции без строк, только дата/название
                // (можно оставить Elements пустым)
                break;

            default:
                throw new ValidationException($"Operation type {dto.Type} not supported yet");
        }
    }

    private void ValidateProductionsBelongToDepartament(string departamentId, List<ElementDto> elements)
    {
        var productIds = elements
            .Select(x => x.ProductionId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()!
            .ToList();

        var map = storage.GetProductionDepartaments(productIds); // prodId -> departamentId

        foreach (var pid in productIds)
        {
            if (!map.TryGetValue(pid!, out var depId) || string.IsNullOrWhiteSpace(depId))
                throw new ValidationException($"Production not found: {pid}");

            if (!string.Equals(depId, departamentId, StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("Нельзя выпускать продукцию не своего цеха (Production.DepartamentId != Operation.DepartamentId)");
        }
    }

    private List<TransactionLogDto> BuildPostings(OperationDto op)
    {
        // счета для операции
        var needNums = op.Type switch
        {
            OperationType.ActualCosts => new[] { "20", "10" },
            OperationType.ReceiptFromProduction => new[] { "43", "20" },
            OperationType.Sale => new[] { "90", "43", "62" },
            OperationType.AllocateActualCost => new[] { "43", "20" },
            OperationType.WriteOffDeviations => new[] { "90", "43", "20" },
            _ => throw new ValidationException($"Operation type {op.Type} not supported yet")
        };

        var acc = storage.GetAccountIdsByNums(needNums);
        foreach (var n in needNums)
            if (!acc.ContainsKey(n))
                throw new ValidationException($"Chart of account {n} not found");

        // ОП4 / ОП5 — отдельно
        if (op.Type == OperationType.AllocateActualCost)
            return BuildOp4_DistributeActualCost(op, acc);

        if (op.Type == OperationType.WriteOffDeviations)
            return BuildOp5_WriteOffDeviationsTo90(op, acc);

        var logs = new List<TransactionLogDto>();

        if (op.Type == OperationType.ActualCosts)
        {
            // TotalAmountDocument проверен выше (>0)
            logs.Add(new TransactionLogDto
            {
                Id = Guid.NewGuid().ToString(),
                DateOperation = op.DateOperation,
                OperationId = op.Id,
                ChartOfAccountDebId = acc["20"],
                ChartOfAccountCredId = acc["10"],
                Subconto1Deb = op.DepartamentId,
                Amount = op.TotalAmountDocument,
                Count = 0,
                Comment = string.IsNullOrWhiteSpace(op.Comment)
                    ? "Накопление фактических затрат Дт20 Кт10"
                    : op.Comment,
                IsDeleted = false
            });
            return logs;
        }

        // planned cost нужен для Receipt/Sale
        var productIds = (op.Elements ?? new())
            .Select(x => x.ProductionId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()!
            .ToList();

        var planned = storage.GetPlannedCostsByProductIds(productIds!);

        if (op.Type == OperationType.ReceiptFromProduction)
        {
            foreach (var e in op.Elements)
            {
                var pc = planned.TryGetValue(e.ProductionId!, out var v) ? v : 0m;
                var sum = e.CountElement * pc;

                logs.Add(new TransactionLogDto
                {
                    Id = Guid.NewGuid().ToString(),
                    DateOperation = op.DateOperation,
                    OperationId = op.Id,
                    ChartOfAccountDebId = acc["43"],
                    ChartOfAccountCredId = acc["20"],
                    Subconto1Deb = e.ProductionId,
                    Subconto1Cred = op.DepartamentId,
                    Amount = sum,
                    Count = e.CountElement,
                    Comment = string.IsNullOrWhiteSpace(op.Comment)
                        ? "Поступление готовой продукции Дт43 Кт20 (плановая)"
                        : op.Comment,
                    IsDeleted = false
                });
            }
            return logs;
        }

        if (op.Type == OperationType.Sale)
        {
            foreach (var e in op.Elements)
            {
                var pc = planned.TryGetValue(e.ProductionId!, out var v) ? v : 0m;
                var planCogs = e.CountElement * pc;
                var revenue = e.CountElement * e.Price!.Value;

                logs.Add(new TransactionLogDto
                {
                    Id = Guid.NewGuid().ToString(),
                    DateOperation = op.DateOperation,
                    OperationId = op.Id,
                    ChartOfAccountDebId = acc["90"],
                    ChartOfAccountCredId = acc["43"],
                    Subconto1Cred = e.ProductionId,
                    Amount = planCogs,
                    Count = e.CountElement,
                    Comment = string.IsNullOrWhiteSpace(op.Comment)
                        ? "Реализация: списание себестоимости Дт90 Кт43 (плановая)"
                        : op.Comment,
                    IsDeleted = false
                });

                logs.Add(new TransactionLogDto
                {
                    Id = Guid.NewGuid().ToString(),
                    DateOperation = op.DateOperation,
                    OperationId = op.Id,
                    ChartOfAccountDebId = acc["62"],
                    ChartOfAccountCredId = acc["90"],
                    Subconto1Deb = op.OrganisationId,
                    Amount = revenue,
                    Count = e.CountElement,
                    Comment = string.IsNullOrWhiteSpace(op.Comment)
                        ? "Реализация: начисление выручки Дт62 Кт90"
                        : op.Comment,
                    IsDeleted = false
                });
            }
            return logs;
        }

        return logs;
    }

    private List<TransactionLogDto> BuildOp4_DistributeActualCost(OperationDto op, Dictionary<string, string> acc)
    {
        var (from, to) = MonthRangeUtc(op.DateOperation);

        var acc43 = acc["43"];
        var acc20 = acc["20"];

        var receipts = storage.GetReceipts43_20_Plan(from, to, acc43, acc20);
        if (receipts.Count == 0)
            throw new ValidationException("Операция 4: нет поступлений Дт43 Кт20 за месяц");

        var do20 = storage.GetDebitTurnover20(from, to, acc20);
        var sumPlanAll = receipts.Values.Sum(x => x.sum);
        if (sumPlanAll == 0m)
            throw new ValidationException("Операция 4: сумма плановых поступлений = 0");

        var logs = new List<TransactionLogDto>();

        foreach (var kv in receipts)
        {
            var productId = kv.Key;
            var sumPlan = kv.Value.sum;

            var sumFact = (do20 / sumPlanAll) * sumPlan;
            var delta = sumFact - sumPlan;
            if (Math.Abs(delta) < 0.0001m) continue;

            logs.Add(new TransactionLogDto
            {
                Id = Guid.NewGuid().ToString(),
                OperationId = op.Id,
                DateOperation = op.DateOperation,
                ChartOfAccountDebId = acc43,
                ChartOfAccountCredId = acc20,
                Subconto1Deb = productId,
                Amount = delta,
                Count = 0,
                Comment = string.IsNullOrWhiteSpace(op.Comment)
                    ? "Распределение фактической себестоимости: отклонение Дт43 Кт20"
                    : op.Comment,
                IsDeleted = false
            });
        }

        return logs;
    }

    private List<TransactionLogDto> BuildOp5_WriteOffDeviationsTo90(OperationDto op, Dictionary<string, string> acc)
    {
        var (from, to) = MonthRangeUtc(op.DateOperation);

        var acc43 = acc["43"];
        var acc20 = acc["20"];
        var acc90 = acc["90"];

        var receipts = storage.GetReceipts43_20_Plan(from, to, acc43, acc20);
        var alloc = storage.GetAllocDeltas43_20(from, to, acc43, acc20);
        var sales = storage.GetSalesCogs90_43_Plan(from, to, acc90, acc43);

        if (sales.Count == 0) return new List<TransactionLogDto>();

        var logs = new List<TransactionLogDto>();

        foreach (var s in sales)
        {
            var productId = s.Key;
            var qtySold = s.Value.qty;
            var sumPlanSold = s.Value.sum;

            if (qtySold <= 0) continue;
            if (!receipts.TryGetValue(productId, out var rec)) continue;

            var qtyF = rec.qty;
            var sumPlanReceipts = rec.sum;
            if (qtyF <= 0) continue;

            var deltaAlloc = alloc.TryGetValue(productId, out var da) ? da : 0m;
            var sumFactReceipts = sumPlanReceipts + deltaAlloc;

            var factUnit = sumFactReceipts / qtyF;
            var planUnit = sumPlanSold / qtySold;

            var deltaSold = (factUnit - planUnit) * qtySold;
            if (Math.Abs(deltaSold) < 0.0001m) continue;

            logs.Add(new TransactionLogDto
            {
                Id = Guid.NewGuid().ToString(),
                OperationId = op.Id,
                DateOperation = op.DateOperation,
                ChartOfAccountDebId = acc90,
                ChartOfAccountCredId = acc43,
                Subconto1Cred = productId,
                Amount = deltaSold,
                Count = 0,
                Comment = string.IsNullOrWhiteSpace(op.Comment)
                    ? "Списание отклонений фактической себестоимости реализованной продукции: Дт90 Кт43"
                    : op.Comment,
                IsDeleted = false
            });
        }

        return logs;
    }

    private static (DateTime from, DateTime to) MonthRangeUtc(DateTime dt)
    {
        var from = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddMonths(1).AddTicks(-1);
        return (from, to);
    }
}
