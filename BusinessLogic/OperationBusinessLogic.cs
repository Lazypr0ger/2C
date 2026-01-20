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
        => storage.GetAll(from, to) ?? throw new NullListException();

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
        NormalizeAndValidateHeader(dto, isCreate: true);
        ValidateByType(dto);

        // построить проводки (внутри тоже есть проверки)
        var logs = BuildPostings(dto);

        // итог
        dto.TotalAmountDocument = logs.Sum(x => x.Amount);

        // атомарное сохранение (шапка+строки+проводки)
        storage.CreateDocument(dto, dto.Elements ?? new(), logs);

        logger.LogInformation("Operation created. Id={Id}, Type={Type}", dto.Id, dto.Type);
    }

    public void Update(OperationDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ValidationException("Operation id is empty");

        NormalizeAndValidateHeader(dto, isCreate: false);
        ValidateByType(dto);

        var logs = BuildPostings(dto);
        dto.TotalAmountDocument = logs.Sum(x => x.Amount);

        storage.UpdateDocument(dto, dto.Elements ?? new(), logs);

        logger.LogInformation("Operation updated. Id={Id}, Type={Type}", dto.Id, dto.Type);
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

    private static void NormalizeAndValidateHeader(OperationDto dto, bool isCreate)
    {
        if (string.IsNullOrWhiteSpace(dto.NameDocument))
            throw new ValidationException("NameDocument is empty");

        // важное: фиксируем DateOperation как UTC, не оставляем Unspecified
        if (dto.DateOperation == default)
            dto.DateOperation = DateTime.UtcNow;

        if (dto.DateOperation.Kind == DateTimeKind.Unspecified)
            dto.DateOperation = DateTime.SpecifyKind(dto.DateOperation, DateTimeKind.Utc);
        else if (dto.DateOperation.Kind == DateTimeKind.Local)
            dto.DateOperation = dto.DateOperation.ToUniversalTime();

        dto.IsDeleted = false;

        // Comment может быть пустым, но пусть будет не null (чтобы фронт не падал)
        dto.Comment ??= string.Empty;

        // элементы тоже не null
        dto.Elements ??= new List<ElementDto>();
    }

    private static void ValidateByType(OperationDto op)
    {
        switch (op.Type)
        {
            case OperationType.ActualCosts:
                if (string.IsNullOrWhiteSpace(op.DepartamentId))
                    throw new ValidationException("DepartamentId is empty for ActualCosts");
                if (op.TotalAmountDocument <= 0m)
                    throw new ValidationException("TotalAmountDocument must be > 0 for ActualCosts");
                break;

            case OperationType.ReceiptFromProduction:
                if (string.IsNullOrWhiteSpace(op.DepartamentId))
                    throw new ValidationException("DepartamentId is empty for ReceiptFromProduction");
                if (op.Elements == null || op.Elements.Count == 0)
                    throw new ValidationException("Elements are empty for ReceiptFromProduction");
                break;

            case OperationType.Sale:
                if (string.IsNullOrWhiteSpace(op.OrganisationId))
                    throw new ValidationException("OrganisationId is empty for Sale");
                if (op.Elements == null || op.Elements.Count == 0)
                    throw new ValidationException("Elements are empty for Sale");
                break;

            case OperationType.AllocateActualCost:
            case OperationType.WriteOffDeviations:
                // эти операции обычно без строк
                break;

            default:
                throw new ValidationException($"Operation type {op.Type} not supported");
        }
    }

    private List<TransactionLogDto> BuildPostings(OperationDto op)
    {
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

        // Комментарий документа (пользовательский) — добавим хвостом в комментарий проводки
        string DocTail() => string.IsNullOrWhiteSpace(op.Comment) ? "" : $" | {op.Comment}";

        if (op.Type == OperationType.ActualCosts)
        {
            if (op.TotalAmountDocument <= 0m)
                throw new ValidationException("TotalAmountDocument must be > 0 for ActualCosts");

            return new List<TransactionLogDto>
            {
                new TransactionLogDto
                {
                    Id = Guid.NewGuid().ToString(),
                    DateOperation = op.DateOperation,
                    OperationId = op.Id,
                    ChartOfAccountDebId = acc["20"],
                    ChartOfAccountCredId = acc["10"],
                    Subconto1Deb = op.DepartamentId,
                    Amount = op.TotalAmountDocument,
                    Count = 0,
                    Comment = "Накопление фактических затрат Дт20 Кт10" + DocTail(),
                    IsDeleted = false
                }
            };
        }

        var productIds = (op.Elements ?? new())
            .Select(x => x.ProductionId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()!;

        var planned = storage.GetPlannedCostsByProductIds(productIds!);

        if (op.Type == OperationType.ReceiptFromProduction)
        {
            var logs = new List<TransactionLogDto>();

            foreach (var e in op.Elements)
            {
                if (string.IsNullOrWhiteSpace(e.ProductionId))
                    throw new ValidationException("Element.ProductionId is empty");
                if (e.CountElement <= 0)
                    throw new ValidationException("Element.CountElement must be > 0");

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
                    Comment = "Поступление готовой продукции Дт43 Кт20 (плановая)" + DocTail(),
                    IsDeleted = false
                });
            }

            return logs;
        }

        if (op.Type == OperationType.Sale)
        {
            var logs = new List<TransactionLogDto>();

            foreach (var e in op.Elements)
            {
                if (string.IsNullOrWhiteSpace(e.ProductionId))
                    throw new ValidationException("Element.ProductionId is empty");
                if (e.CountElement <= 0)
                    throw new ValidationException("Element.CountElement must be > 0");
                if (e.Price is null || e.Price <= 0)
                    throw new ValidationException("Element.Price must be > 0 for Sale");

                var pc = planned.TryGetValue(e.ProductionId!, out var v) ? v : 0m;
                var planCogs = e.CountElement * pc;
                var revenue = e.CountElement * e.Price.Value;

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
                    Comment = "Реализация: списание себестоимости Дт90 Кт43 (плановая)" + DocTail(),
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
                    Comment = "Реализация: начисление выручки Дт62 Кт90" + DocTail(),
                    IsDeleted = false
                });
            }

            return logs;
        }

        if (op.Type == OperationType.AllocateActualCost)
            return BuildOp4_DistributeActualCost(op, acc);

        if (op.Type == OperationType.WriteOffDeviations)
            return BuildOp5_WriteOffDeviationsTo90(op, acc);

        return new List<TransactionLogDto>();
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
                Comment = "Распределение фактической себестоимости: отклонение Дт43 Кт20" +
                          (string.IsNullOrWhiteSpace(op.Comment) ? "" : $" | {op.Comment}"),
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
                Comment = "Списание отклонений фактической себестоимости реализованной продукции: Дт90 Кт43" +
                          (string.IsNullOrWhiteSpace(op.Comment) ? "" : $" | {op.Comment}"),
                IsDeleted = false
            });
        }

        return logs;
    }

    private static (DateTime from, DateTime to) MonthRangeUtc(DateTime dt)
    {
        var utc = dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        var from = new DateTime(utc.Year, utc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddMonths(1).AddTicks(-1);
        return (from, to);
    }
}
