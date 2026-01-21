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

        // ✅ Месячные ограничения по операциям 4/5
        ValidateMonthlyRulesOnCreate(dto);

        // ✅ Ограничения "остатков" (без хранения в БД)
        ValidateComputedBalances(dto, oldForUpdate: null);

        var logs = BuildPostings(dto);

        var sumLogs = logs.Sum(x => x.Amount);
        if (dto.Type == OperationType.ActualCosts)
        {
            if (sumLogs != dto.TotalAmountDocument)
                throw new ValidationException("ActualCosts: postings sum mismatch with TotalAmountDocument");
        }
        else
        {
            dto.TotalAmountDocument = sumLogs;
        }

        storage.CreateDocument(dto, dto.Elements ?? new(), logs);
        logger.LogInformation("Operation created. Id={Id}, Type={Type}", dto.Id, dto.Type);
    }

    public void Update(OperationDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ValidationException("Operation id is empty");

        var old = storage.GetById(dto.Id) ?? throw new ElementNotFoundException(dto.Id);

        NormalizeAndValidateHeader(dto, isCreate: false);
        ValidateByType(dto);

        // ✅ Месячные ограничения по операциям 4/5 (учитываем, что это update существующей)
        ValidateMonthlyRulesOnUpdate(dto, old);

        // ✅ Ограничения "остатков" (без хранения в БД) — с поправкой на старую операцию
        ValidateComputedBalances(dto, oldForUpdate: old);

        var logs = BuildPostings(dto);
        dto.TotalAmountDocument = logs.Sum(x => x.Amount);

        storage.UpdateDocument(dto, dto.Elements ?? new(), logs);
        logger.LogInformation("Operation updated. Id={Id}, Type={Type}", dto.Id, dto.Type);
    }

    public void Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Operation id is empty");

        // ✅ storage должен помечать удалёнными и проводки (ты это уже внес)
        storage.Delete(id);
    }

    public void Recovery(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Operation id is empty");

        // ✅ storage должен восстанавливать и проводки (если так задумано)
        storage.Recovery(id);
    }

    // =========================================================
    // Header / validation
    // =========================================================

    private static void NormalizeAndValidateHeader(OperationDto dto, bool isCreate)
    {
        if (string.IsNullOrWhiteSpace(dto.NameDocument))
            throw new ValidationException("NameDocument is empty");

        if (dto.DateOperation == default)
            dto.DateOperation = DateTime.UtcNow;

        if (dto.DateOperation.Kind == DateTimeKind.Unspecified)
            dto.DateOperation = DateTime.SpecifyKind(dto.DateOperation, DateTimeKind.Utc);
        else if (dto.DateOperation.Kind == DateTimeKind.Local)
            dto.DateOperation = dto.DateOperation.ToUniversalTime();

        if (isCreate)
            dto.IsDeleted = false;

        dto.Comment ??= string.Empty;
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

    // =========================================================
    // ✅ Monthly rules for op4 / op5
    // =========================================================

    private void ValidateMonthlyRulesOnCreate(OperationDto dto)
    {
        if (dto.Type != OperationType.AllocateActualCost && dto.Type != OperationType.WriteOffDeviations)
            return;

        var (from, to) = MonthRangeUtc(dto.DateOperation);

        if (dto.Type == OperationType.AllocateActualCost)
        {
            if (storage.ExistsMonthlyOperation(OperationType.AllocateActualCost, from, to))
            {
                var id = storage.FindMonthlyOperationId(OperationType.AllocateActualCost, from, to);
                throw new ValidationException($"Операция распределения (4) уже проведена за этот месяц. Обновите существующую. Id={id}");
            }
        }

        if (dto.Type == OperationType.WriteOffDeviations)
        {
            if (!storage.ExistsMonthlyOperation(OperationType.AllocateActualCost, from, to))
            {
                throw new ValidationException("Нельзя выполнить списание отклонений (5), пока не выполнено распределение (4) за этот месяц.");
            }

            if (storage.ExistsMonthlyOperation(OperationType.WriteOffDeviations, from, to))
            {
                var id = storage.FindMonthlyOperationId(OperationType.WriteOffDeviations, from, to);
                throw new ValidationException($"Операция списания отклонений (5) уже проведена за этот месяц. Обновите существующую. Id={id}");
            }
        }
    }

    private void ValidateMonthlyRulesOnUpdate(OperationDto dto, OperationDto old)
    {
        if (dto.Type != OperationType.AllocateActualCost && dto.Type != OperationType.WriteOffDeviations)
            return;

        var (from, to) = MonthRangeUtc(dto.DateOperation);

        if (dto.Type == OperationType.AllocateActualCost)
        {
            // Если в этом месяце есть операция 4, то она должна быть именно эта (dto.Id)
            var existingId = storage.FindMonthlyOperationId(OperationType.AllocateActualCost, from, to);
            if (!string.IsNullOrWhiteSpace(existingId) && existingId != dto.Id)
                throw new ValidationException($"Операция распределения (4) уже проведена за этот месяц. Обновите существующую. Id={existingId}");
        }

        if (dto.Type == OperationType.WriteOffDeviations)
        {
            // Требуем наличие операции 4 в этом месяце (может быть эта же операция? нет, тип другой)
            var allocId = storage.FindMonthlyOperationId(OperationType.AllocateActualCost, from, to);
            if (string.IsNullOrWhiteSpace(allocId))
                throw new ValidationException("Нельзя выполнить списание отклонений (5), пока не выполнено распределение (4) за этот месяц.");

            // И в этом месяце операция 5 должна быть именно эта
            var existingId = storage.FindMonthlyOperationId(OperationType.WriteOffDeviations, from, to);
            if (!string.IsNullOrWhiteSpace(existingId) && existingId != dto.Id)
                throw new ValidationException($"Операция списания отклонений (5) уже проведена за этот месяц. Обновите существующую. Id={existingId}");
        }
    }

    // =========================================================
    // ✅ Computed balances validation (no DB stocks)
    // =========================================================

    private void ValidateComputedBalances(OperationDto dto, OperationDto? oldForUpdate)
    {
        switch (dto.Type)
        {
            case OperationType.ReceiptFromProduction:
                ValidateProductionCapacity(dto, oldForUpdate);
                break;

            case OperationType.Sale:
                ValidateSaleStock(dto, oldForUpdate);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// ReceiptFromProduction:
    /// (Материалы 20-10 по подразделению) - (Произведено 43-20 по подразделению, в плановой оценке) >= (плановая оценка выпуска текущей операции)
    /// </summary>
    private void ValidateProductionCapacity(OperationDto dto, OperationDto? oldForUpdate)
    {
        if (string.IsNullOrWhiteSpace(dto.DepartamentId))
            throw new ValidationException("DepartamentId is empty for ReceiptFromProduction");

        var acc = storage.GetAccountIdsByNums(new[] { "20", "10", "43" });
        if (!acc.TryGetValue("20", out var acc20) ||
            !acc.TryGetValue("10", out var acc10) ||
            !acc.TryGetValue("43", out var acc43))
            throw new ValidationException("Accounts 20/10/43 not found in ChartOfAccount");

        var productIds = (dto.Elements ?? new())
            .Select(e => e.ProductionId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList()!;

        if (productIds.Count == 0)
            throw new ValidationException("ReceiptFromProduction: product ids are empty");

        var planned = storage.GetPlannedCostsByProductIds(productIds);

        decimal NewOpPlanCost()
        {
            decimal sum = 0m;
            foreach (var e in dto.Elements ?? new())
            {
                if (string.IsNullOrWhiteSpace(e.ProductionId))
                    throw new ValidationException("Element.ProductionId is empty");
                if (e.CountElement <= 0)
                    throw new ValidationException("Element.CountElement must be > 0");

                var pc = planned.TryGetValue(e.ProductionId!, out var v) ? v : 0m;
                sum += pc * e.CountElement;
            }
            return sum;
        }

        var needed = NewOpPlanCost();

        // агрегаты "на дату операции"
        var to = dto.DateOperation;
        var materials = storage.GetMaterialsInput20_10_Department(to, acc20, acc10, dto.DepartamentId!);
        var produced = storage.GetProducedPlanCost43_20_Department(to, acc43, acc20, dto.DepartamentId!);

        // ✅ поправка для Update: вернём вклад старой операции, т.к. produced уже включает её
        if (oldForUpdate != null
            && oldForUpdate.Type == OperationType.ReceiptFromProduction
            && !oldForUpdate.IsDeleted
            && oldForUpdate.DepartamentId == dto.DepartamentId
            && oldForUpdate.DateOperation <= to)
        {
            // считаем плановую сумму старой операции
            var oldIds = (oldForUpdate.Elements ?? new())
                .Select(e => e.ProductionId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList()!;

            var oldPlanned = storage.GetPlannedCostsByProductIds(oldIds);

            decimal oldSum = 0m;
            foreach (var e in oldForUpdate.Elements ?? new())
            {
                if (string.IsNullOrWhiteSpace(e.ProductionId)) continue;
                var pc = oldPlanned.TryGetValue(e.ProductionId!, out var v) ? v : 0m;
                oldSum += pc * e.CountElement;
            }

            produced -= oldSum;
        }

        var available = materials - produced;

        if (available < needed - 0.0001m)
            throw new ValidationException(
                $"Недостаточно материалов для выпуска. Доступно: {available:N2}, требуется: {needed:N2}.");
    }

    /// <summary>
    /// Sale:
    /// (Произведено qty 43-20) - (Продано qty 90-43) >= (qty продажи по каждому продукту)
    /// </summary>
    private void ValidateSaleStock(OperationDto dto, OperationDto? oldForUpdate)
    {
        var acc = storage.GetAccountIdsByNums(new[] { "43", "20", "90" });
        if (!acc.TryGetValue("43", out var acc43) ||
            !acc.TryGetValue("20", out var acc20) ||
            !acc.TryGetValue("90", out var acc90))
            throw new ValidationException("Accounts 43/20/90 not found in ChartOfAccount");

        var productIds = (dto.Elements ?? new())
            .Select(e => e.ProductionId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList()!;

        if (productIds.Count == 0)
            throw new ValidationException("Sale: product ids are empty");

        var to = dto.DateOperation;

        var producedQty = storage.GetProducedQty43_20_ByProduct(to, acc43, acc20, productIds);
        var soldQty = storage.GetSoldQty90_43_ByProduct(to, acc90, acc43, productIds);

        // ✅ поправка для Update: вернём вклад старой операции продажи, т.к. soldQty уже включает её
        Dictionary<string, int> oldSoldByProduct = new();
        if (oldForUpdate != null
            && oldForUpdate.Type == OperationType.Sale
            && !oldForUpdate.IsDeleted
            && oldForUpdate.DateOperation <= to)
        {
            foreach (var e in oldForUpdate.Elements ?? new())
            {
                if (string.IsNullOrWhiteSpace(e.ProductionId)) continue;
                if (e.CountElement <= 0) continue;

                oldSoldByProduct.TryGetValue(e.ProductionId!, out var cur);
                oldSoldByProduct[e.ProductionId!] = cur + e.CountElement;
            }
        }

        foreach (var e in dto.Elements ?? new())
        {
            if (string.IsNullOrWhiteSpace(e.ProductionId))
                throw new ValidationException("Element.ProductionId is empty");
            if (e.CountElement <= 0)
                throw new ValidationException("Element.CountElement must be > 0");

            var pid = e.ProductionId!;
            var made = producedQty.TryGetValue(pid, out var mq) ? mq : 0;
            var sold = soldQty.TryGetValue(pid, out var sq) ? sq : 0;

            if (oldSoldByProduct.TryGetValue(pid, out var oldSold))
                sold -= oldSold; // откатываем старую версию операции

            var available = made - sold;

            if (available < e.CountElement)
                throw new ValidationException(
                    $"Недостаточно остатка для продажи по продукции {pid}. Доступно: {available}, требуется: {e.CountElement}.");
        }
    }

    // =========================================================
    // Postings builder (как у тебя было)
    // =========================================================

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
            var depMap = storage.GetProductionDepartaments(productIds!);

            foreach (var pid in productIds!)
            {
                if (!depMap.TryGetValue(pid, out var depId))
                    throw new ValidationException($"Production not found: {pid}");

                if (depId != op.DepartamentId)
                    throw new ValidationException($"Production {pid} does not belong to departament {op.DepartamentId}");
            }

            foreach (var e in op.Elements ?? new())
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

            foreach (var e in op.Elements ?? new())
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
