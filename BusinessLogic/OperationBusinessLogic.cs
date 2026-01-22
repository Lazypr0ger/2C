using Contracts.DTO;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace BusinessLogic;

public class OperationBusinessLogic(
    IOperationStorageContract storage,
    ILogger<OperationBusinessLogic> logger) : IOperationBusinessLogic
{
    // Константы для валидации
    private const decimal MaxSingleElementAmount = 1_000_000_000m;
    private const decimal MaxTotalOperationAmount = 100_000_000m;
    private const int MaxNameLength = 200;
    private const int MaxCommentLength = 1000;
    private const int MaxElementsCount = 1000;
    private const int MaxCountElement = 1_000_000_000;

    public List<OperationDto> GetAll(DateTime? from = null, DateTime? to = null)
    {
        try
        {
            // Валидация диапазона дат
            if (from.HasValue && to.HasValue && from.Value > to.Value)
            {
                throw new ValidationException("Дата 'от' не может быть позже даты 'до'");
            }

            // Ограничение на слишком большой диапазон запроса (например, больше 10 лет)
            if (from.HasValue && to.HasValue)
            {
                var range = to.Value - from.Value;
                if (range.TotalDays > 3650) // 10 лет
                {
                    throw new ValidationException("Диапазон запроса не может превышать 10 лет");
                }
            }

            return storage.GetAll(from, to) ?? throw new NullListException();
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении списка операций");
            throw new ValidationException("Не удалось получить список операций");
        }
    }

    public OperationDto GetById(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ValidationException("ID операции не может быть пустым");

            if (id.Length > 100)
                throw new ValidationException("ID операции слишком длинный");

            // Проверка формата GUID (если используется GUID)
            if (!Guid.TryParse(id, out _) && id.Length != 36)
            {
                logger.LogWarning("Передан некорректный формат ID операции: {Id}", id);
            }

            var result = storage.GetById(id);
            if (result == null)
                throw new ElementNotFoundException(id);

            return result;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (ElementNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении операции по ID: {Id}", id);
            throw new ValidationException("Не удалось получить операцию");
        }
    }

    public void Create(OperationDto dto)
    {
        try
        {
            ValidateOperationDto(dto, isCreate: true);

            dto.Id ??= Guid.NewGuid().ToString();
            NormalizeAndValidateHeader(dto, isCreate: true);
            ValidateByType(dto);

            // Дополнительная валидация элементов
            ValidateElements(dto.Elements ?? new List<ElementDto>(), dto.Type);

            // Проверка общей суммы
            ValidateTotalAmount(dto);

            // Месячные ограничения по операциям 4/5
            ValidateMonthlyRulesOnCreate(dto);

            // Ограничения "остатков" (без хранения в БД)
            ValidateComputedBalances(dto, oldForUpdate: null);

            var logs = BuildPostings(dto);

            var sumLogs = logs.Sum(x => x.Amount);
            if (dto.Type == OperationType.ActualCosts)
            {
                if (Math.Abs(sumLogs - dto.TotalAmountDocument) > 0.01m)
                    throw new ValidationException("ActualCosts: сумма проводок не соответствует TotalAmountDocument");
            }
            else
            {
                dto.TotalAmountDocument = sumLogs;
            }

            storage.CreateDocument(dto, dto.Elements ?? new(), logs);
            logger.LogInformation("Operation created. Id={Id}, Type={Type}", dto.Id, dto.Type);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при создании операции. Type: {Type}", dto?.Type);
            throw new ValidationException("Не удалось создать операцию");
        }
    }

    public void Update(OperationDto dto)
    {
        try
        {
            ValidateOperationDto(dto, isCreate: false);

            if (string.IsNullOrWhiteSpace(dto.Id))
                throw new ValidationException("ID операции не может быть пустым");

            var old = storage.GetById(dto.Id) ?? throw new ElementNotFoundException(dto.Id);

            NormalizeAndValidateHeader(dto, isCreate: false);
            ValidateByType(dto);

            // Дополнительная валидация элементов
            ValidateElements(dto.Elements ?? new List<ElementDto>(), dto.Type);

            // Проверка общей суммы
            ValidateTotalAmount(dto);

            // Месячные ограничения по операциям 4/5 (учитываем, что это update существующей)
            ValidateMonthlyRulesOnUpdate(dto, old);

            //  Ограничения "остатков" (без хранения в БД) — с поправкой на старую операцию
            ValidateComputedBalances(dto, oldForUpdate: old);

            var logs = BuildPostings(dto);
            dto.TotalAmountDocument = logs.Sum(x => x.Amount);

            storage.UpdateDocument(dto, dto.Elements ?? new(), logs);
            logger.LogInformation("Operation updated. Id={Id}, Type={Type}", dto.Id, dto.Type);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (ElementNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обновлении операции. Id: {Id}, Type: {Type}", dto?.Id, dto?.Type);
            throw new ValidationException("Не удалось обновить операцию");
        }
    }

    public void Delete(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ValidationException("ID операции не может быть пустым");

            // Проверяем существование операции перед удалением
            var operation = storage.GetById(id);
            if (operation == null)
                throw new ElementNotFoundException(id);

            // storage должен помечать удалёнными и проводки
            storage.Delete(id);
            logger.LogInformation("Operation deleted. Id={Id}", id);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (ElementNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при удалении операции. Id: {Id}", id);
            throw new ValidationException("Не удалось удалить операцию");
        }
    }

    public void Recovery(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ValidationException("ID операции не может быть пустым");

            //  storage должен восстанавливать и проводки
            storage.Recovery(id);
            logger.LogInformation("Operation recovered. Id={Id}", id);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при восстановлении операции. Id: {Id}", id);
            throw new ValidationException("Не удалось восстановить операцию");
        }
    }

    // Валидация DTO


    private void ValidateOperationDto(OperationDto dto, bool isCreate)
    {
        if (dto is null)
            throw new ArgumentNullException(nameof(dto));

        // Валидация ID при обновлении
        if (!isCreate && string.IsNullOrWhiteSpace(dto.Id))
            throw new ValidationException("ID операции не может быть пустым при обновлении");
    }

    // Header / validation
 
    private static void NormalizeAndValidateHeader(OperationDto dto, bool isCreate)
    {
        if (string.IsNullOrWhiteSpace(dto.NameDocument))
            throw new ValidationException("Название документа не может быть пустым");

        dto.NameDocument = dto.NameDocument.Trim();
        if (dto.NameDocument.Length > MaxNameLength)
            throw new ValidationException($"Название документа не может превышать {MaxNameLength} символов");

        if (dto.DateOperation == default)
            dto.DateOperation = DateTime.UtcNow;

        if (dto.DateOperation.Kind == DateTimeKind.Unspecified)
            dto.DateOperation = DateTime.SpecifyKind(dto.DateOperation, DateTimeKind.Utc);
        else if (dto.DateOperation.Kind == DateTimeKind.Local)
            dto.DateOperation = dto.DateOperation.ToUniversalTime();

        // Проверка даты
        var now = DateTime.UtcNow;
        if (dto.DateOperation > now.AddDays(1))
            throw new ValidationException("Дата операции не может быть в будущем больше чем на 1 день");

        if (dto.DateOperation < new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            throw new ValidationException("Дата операции не может быть раньше 2000 года");

        if (isCreate)
            dto.IsDeleted = false;
        else if (dto.IsDeleted)
            throw new ValidationException("Нельзя обновить удаленную операцию");

        dto.Comment ??= string.Empty;
        dto.Comment = dto.Comment.Trim();
        if (dto.Comment.Length > MaxCommentLength)
            throw new ValidationException($"Комментарий не может превышать {MaxCommentLength} символов");

        dto.Elements ??= new List<ElementDto>();
    }

    private static void ValidateByType(OperationDto op)
    {
        switch (op.Type)
        {
            case OperationType.ActualCosts:
                if (string.IsNullOrWhiteSpace(op.DepartamentId))
                    throw new ValidationException("Для ActualCosts необходимо указать подразделение");
                if (op.TotalAmountDocument <= 0m)
                    throw new ValidationException("TotalAmountDocument должен быть больше 0 для ActualCosts");
                if (op.TotalAmountDocument > MaxTotalOperationAmount)
                    throw new ValidationException($"Общая сумма операции не может превышать {MaxTotalOperationAmount:N0}");
                break;

            case OperationType.ReceiptFromProduction:
                if (string.IsNullOrWhiteSpace(op.DepartamentId))
                    throw new ValidationException("Для ReceiptFromProduction необходимо указать подразделение");
                if (op.Elements == null || op.Elements.Count == 0)
                    throw new ValidationException("Для ReceiptFromProduction необходимо указать хотя бы один элемент");
                if (op.Elements.Count > MaxElementsCount)
                    throw new ValidationException($"Количество элементов не может превышать {MaxElementsCount}");
                break;

            case OperationType.Sale:
                if (string.IsNullOrWhiteSpace(op.OrganisationId))
                    throw new ValidationException("Для Sale необходимо указать организацию");
                if (op.Elements == null || op.Elements.Count == 0)
                    throw new ValidationException("Для Sale необходимо указать хотя бы один элемент");
                if (op.Elements.Count > MaxElementsCount)
                    throw new ValidationException($"Количество элементов не может превышать {MaxElementsCount}");
                break;

            case OperationType.AllocateActualCost:
            case OperationType.WriteOffDeviations:
 
                if (op.Elements != null && op.Elements.Count > 0)
                    throw new ValidationException($"Для операции типа {op.Type} не должно быть элементов");
                break;

            default:
                throw new ValidationException($"Тип операции {op.Type} не поддерживается");
        }
    }

    // Валидация элементов


    private void ValidateElements(List<ElementDto> elements, OperationType operationType)
    {
        if (elements == null)
            return;

        var seenProductIds = new HashSet<string>();

        foreach (var element in elements)
        {
            // Проверка ProductionId
            if (string.IsNullOrWhiteSpace(element.ProductionId))
                throw new ValidationException("ProductionId элемента не может быть пустым");

            if (element.ProductionId.Length > 100)
                throw new ValidationException("ProductionId элемента слишком длинный");

            // Проверка уникальности ProductionId в рамках одной операции
            if (!seenProductIds.Add(element.ProductionId))
                throw new ValidationException($"ProductionId '{element.ProductionId}' повторяется в операции");

            // Проверка CountElement
            if (element.CountElement <= 0)
                throw new ValidationException("Количество элемента должно быть больше 0");

            if (element.CountElement > MaxCountElement)
                throw new ValidationException($"Количество элемента не может превышать {MaxCountElement:N0}");

            // Проверка Price в зависимости от типа операции
            if (operationType == OperationType.Sale)
            {
                if (element.Price == null)
                    throw new ValidationException("Для Sale необходимо указать цену элемента");

                if (element.Price <= 0m)
                    throw new ValidationException("Цена элемента должна быть больше 0");

                if (element.Price > MaxSingleElementAmount)
                    throw new ValidationException($"Цена элемента не может превышать {MaxSingleElementAmount:N0}");

                var elementAmount = element.CountElement * element.Price.Value;
                if (elementAmount > MaxSingleElementAmount)
                    throw new ValidationException($"Сумма по элементу не может превышать {MaxSingleElementAmount:N0}");
            }
            else if (operationType == OperationType.ReceiptFromProduction)
            {
                if (element.Price != null && element.Price > 0m)
                    throw new ValidationException("Для ReceiptFromProduction цена должна быть равна 0 или null");
            }
        }
    }


    // Валидация общей суммы


    private void ValidateTotalAmount(OperationDto dto)
    {
        if (dto.Type == OperationType.ActualCosts)
        {
            if (dto.TotalAmountDocument > MaxTotalOperationAmount)
                throw new ValidationException($"Общая сумма операции не может превышать {MaxTotalOperationAmount:N0}. Проведите несколько операций.");
        }
        else if (dto.Type == OperationType.Sale)
        {
            decimal total = 0m;
            foreach (var element in dto.Elements ?? new())
            {
                if (element.Price == null)
                    throw new ValidationException("Для Sale необходимо указать цену элемента");

                var elementAmount = element.CountElement * element.Price.Value;
                total += elementAmount;

                if (total > MaxTotalOperationAmount)
                    throw new ValidationException($"Общая сумма операции не может превышать {MaxTotalOperationAmount:N0}. Проведите несколько операций.");
            }
        }
    }


    //  Monthly rules for op4 / op5
  

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


    // Computed balances validation 

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


    /// ReceiptFromProduction:
    /// (Материалы 20-10 по подразделению) - (Произведено 43-20 по подразделению, в плановой оценке) >= (плановая оценка выпуска текущей операции)
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

    /// Sale:
    /// (Произведено qty 43-20) - (Продано qty 90-43) >= (qty продажи по каждому продукту)

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

    // Postings builder 

    private List<TransactionLogDto> BuildPostings(OperationDto op)
    {
        try
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
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при построении проводок для операции Type: {Type}", op.Type);
            throw new ValidationException("Не удалось построить проводки для операции");
        }
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