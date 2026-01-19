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

        if (string.IsNullOrWhiteSpace(dto.NameDocument))
            throw new ValidationException("NameDocument is empty");

        if (dto.DateOperation == default)
            dto.DateOperation = DateTime.UtcNow;

        dto.IsDeleted = false;

        // 1) сформировать проводки заранее (чтобы знать сумму)
        var logs = BuildPostings(dto);

        // 2) итог документа
        dto.TotalAmountDocument = logs.Sum(x => x.Amount);

        // 3) сохранить шапку ОДИН раз
        storage.Create(dto);

        // 4) сохранить строки
        storage.ReplaceElements(dto.Id, dto.Elements ?? new());

        // 5) сохранить проводки
        storage.ReplaceTransactionLogs(dto.Id, logs);

        logger.LogInformation("Operation created and posted. Id={Id}, Type={Type}", dto.Id, dto.Type);
    }

    public void Update(OperationDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ValidationException("Operation id is empty");

        if (string.IsNullOrWhiteSpace(dto.NameDocument))
            throw new ValidationException("NameDocument is empty");

        // 1) обновить шапку
        storage.Update(dto);

        // 2) заменить строки
        storage.ReplaceElements(dto.Id, dto.Elements ?? new());

        // 3) пересоздать проводки
        var logs = BuildPostings(dto);
        storage.ReplaceTransactionLogs(dto.Id, logs);

        // 4) пересчитать итог
        dto.TotalAmountDocument = logs.Sum(x => x.Amount);
        storage.Update(dto);

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

    private List<TransactionLogDto> BuildPostings(OperationDto op)
    {
        // какие счета нужны
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

        var logs = new List<TransactionLogDto>();

        if (op.Type == OperationType.ActualCosts)
        {
            // ручной ввод затрат: сейчас делаем простейший вариант — одна проводка на сумму TotalAmountDocument
            if (op.TotalAmountDocument == 0m)
                throw new ValidationException("TotalAmountDocument is empty for ActualCosts");

            logs.Add(new TransactionLogDto
            {
                Id = Guid.NewGuid().ToString(),
                DateOperation = op.DateOperation,
                OperationId = op.Id,
                ChartOfAccountDebId = acc["20"],
                ChartOfAccountCredId = acc["10"],
                Subconto1Deb = op.DepartamentId, // аналитика 20 = подразделение (по ТЗ)
                Amount = op.TotalAmountDocument,
                Count = 0,
                Comment = "Накопление фактических затрат Дт20 Кт10",
                IsDeleted = false
            });
            return logs;
        }

        // для 43/20 и 90/43 нужно PlannedCost по продуктам
        var productIds = (op.Elements ?? new()).Select(x => x.ProductionId).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()!;
        var planned = storage.GetPlannedCostsByProductIds(productIds!);

        if (op.Type == OperationType.ReceiptFromProduction)
        {
            if (string.IsNullOrWhiteSpace(op.DepartamentId))
                throw new ValidationException("DepartamentId is empty for ReceiptFromProduction");

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
                    Subconto1Deb = e.ProductionId,     // 43 аналитика = продукт
                    Subconto1Cred = op.DepartamentId,  // 20 аналитика = подразделение
                    Amount = sum,
                    Count = e.CountElement,
                    Comment = "Поступление готовой продукции Дт43 Кт20 (плановая)",
                    IsDeleted = false
                });
            }

            return logs;
        }

        if (op.Type == OperationType.Sale)
        {
            if (string.IsNullOrWhiteSpace(op.OrganisationId))
                throw new ValidationException("OrganisationId is empty for Sale");

            foreach (var e in op.Elements)
            {
                if (string.IsNullOrWhiteSpace(e.ProductionId))
                    throw new ValidationException("Element.ProductionId is empty");

                if (e.CountElement <= 0)
                    throw new ValidationException("Element.CountElement must be > 0");

                if (e.Price is null || e.Price <= 0)
                    throw new ValidationException("Element.Price must be > 0 for Sale");

                var pc = planned.TryGetValue(e.ProductionId!, out var v) ? v : 0m;
                var planCogs = e.CountElement * pc;          // списание себестоимости
                var revenue = e.CountElement * e.Price.Value; // выручка

                // 1) Дт90 Кт43 (себестоимость)
                logs.Add(new TransactionLogDto
                {
                    Id = Guid.NewGuid().ToString(),
                    DateOperation = op.DateOperation,
                    OperationId = op.Id,
                    ChartOfAccountDebId = acc["90"],
                    ChartOfAccountCredId = acc["43"],
                    Subconto1Cred = e.ProductionId, // 43 аналитика = продукт
                    Amount = planCogs,
                    Count = e.CountElement,
                    Comment = "Реализация: списание себестоимости Дт90 Кт43 (плановая)",
                    IsDeleted = false
                });

                // 2) Дт62 Кт90 (выручка)
                logs.Add(new TransactionLogDto
                {
                    Id = Guid.NewGuid().ToString(),
                    DateOperation = op.DateOperation,
                    OperationId = op.Id,
                    ChartOfAccountDebId = acc["62"],
                    ChartOfAccountCredId = acc["90"],
                    Subconto1Deb = op.OrganisationId, // 62 аналитика = покупатель/контрагент
                    Amount = revenue,
                    Count = e.CountElement,
                    Comment = "Реализация: начисление выручки Дт62 Кт90",
                    IsDeleted = false
                });
               

            }

            return logs;
        }
        if (op.Type == OperationType.AllocateActualCost)
            return BuildOp4_DistributeActualCost(op, acc);

        if (op.Type == OperationType.WriteOffDeviations)
            return BuildOp5_WriteOffDeviationsTo90(op, acc);

        return logs;
    }

    private List<TransactionLogDto> BuildOp4_DistributeActualCost(OperationDto op, Dictionary<string, string> acc)
    {
        var (from, to) = MonthRangeUtc(op.DateOperation);

        var acc43 = acc["43"];
        var acc20 = acc["20"];

        var receipts = storage.GetReceipts43_20_Plan(from, to, acc43, acc20); // product -> (qty, sumPlan)
        if (receipts.Count == 0)
            throw new ValidationException("Операция 4: нет поступлений Дт43 Кт20 за месяц");

        var do20 = storage.GetDebitTurnover20(from, to, acc20); // дебетовый оборот 20
        var sumPlanAll = receipts.Values.Sum(x => x.sum);
        if (sumPlanAll == 0m)
            throw new ValidationException("Операция 4: сумма плановых поступлений = 0");

        var logs = new List<TransactionLogDto>();

        foreach (var kv in receipts)
        {
            var productId = kv.Key;
            var sumPlan = kv.Value.sum;

            // Sum_F(i) = (DO / sumPlanAll) * sumPlan_i
            var sumFact = (do20 / sumPlanAll) * sumPlan;

            // delta = Sum_F - Sum_P
            var delta = sumFact - sumPlan;
            if (Math.Abs(delta) < 0.0001m) continue;

            logs.Add(new TransactionLogDto
            {
                Id = Guid.NewGuid().ToString(),
                OperationId = op.Id,
                DateOperation = op.DateOperation,
                ChartOfAccountDebId = acc43,
                ChartOfAccountCredId = acc20,
                Subconto1Deb = productId, // 43 аналитика по продукту
                Amount = delta,
                Count = 0,
                Comment = "Распределение фактической себестоимости: отклонение Дт43 Кт20",
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

        var receipts = storage.GetReceipts43_20_Plan(from, to, acc43, acc20); // product -> (qtyF, sumPlanReceipts)
        var alloc = storage.GetAllocDeltas43_20(from, to, acc43, acc20);      // product -> deltaAlloc
        var sales = storage.GetSalesCogs90_43_Plan(from, to, acc90, acc43);   // product -> (qtySold, sumPlanCogs)

        if (sales.Count == 0) return new List<TransactionLogDto>(); // нечего списывать

        var logs = new List<TransactionLogDto>();

        foreach (var s in sales)
        {
            var productId = s.Key;
            var qtySold = s.Value.qty;
            var sumPlanSold = s.Value.sum; // плановая себестоимость продаж (из Дт90 Кт43)

            if (qtySold <= 0) continue;

            if (!receipts.TryGetValue(productId, out var rec)) continue;
            var qtyF = rec.qty;
            var sumPlanReceipts = rec.sum;

            if (qtyF <= 0) continue;

            var deltaAlloc = alloc.TryGetValue(productId, out var da) ? da : 0m;
            var sumFactReceipts = sumPlanReceipts + deltaAlloc;

            // Фактическая себестоимость единицы = Sum_F / kol_F
            var factUnit = sumFactReceipts / qtyF;

            // Плановая себестоимость единицы по продажам = Sum_P_sold / kol_P_sold
            var planUnit = sumPlanSold / qtySold;

            // Отклонение при реализации = (factUnit - planUnit) * qtySold
            var deltaSold = (factUnit - planUnit) * qtySold;
            if (Math.Abs(deltaSold) < 0.0001m) continue;

            logs.Add(new TransactionLogDto
            {
                Id = Guid.NewGuid().ToString(),
                OperationId = op.Id,
                DateOperation = op.DateOperation,
                ChartOfAccountDebId = acc90,
                ChartOfAccountCredId = acc43,
                Subconto1Cred = productId, // 43 аналитика по продукту
                Amount = deltaSold,
                Count = 0,
                Comment = "Списание отклонений фактической себестоимости реализованной продукции: Дт90 Кт43",
                IsDeleted = false
            });
        }

        return logs;
    }

    private static (DateTime from, DateTime to) MonthRangeUtc(DateTime dt)
    {
        // У тебя везде UtcNow, так что делаем UTC-границы
        var from = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddMonths(1).AddTicks(-1);
        return (from, to);
    }

}
