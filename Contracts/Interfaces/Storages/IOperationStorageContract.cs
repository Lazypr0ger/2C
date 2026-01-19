using Contracts.DTO;

namespace Contracts.Interfaces.Storages;

public interface IOperationStorageContract
{
    List<OperationDto> GetAll(DateTime? from = null, DateTime? to = null);
    OperationDto GetById(string id);

    void Create(OperationDto dto);
    void Update(OperationDto dto);

    void Delete(string id);
    void Recovery(string id);

    // для проведения:
    void ReplaceElements(string operationId, List<ElementDto> elements);
    void ReplaceTransactionLogs(string operationId, List<TransactionLogDto> logs);

    // справочные данные для формирования проводок
    Dictionary<string, string> GetAccountIdsByNums(IEnumerable<string> nums);
    Dictionary<string, decimal> GetPlannedCostsByProductIds(IEnumerable<string> productIds);

    // Дт43 Кт20 по плану: productId -> (qty, sumPlan)
    Dictionary<string, (int qty, decimal sum)> GetReceipts43_20_Plan(DateTime from, DateTime to, string acc43Id, string acc20Id);

    // Дт43 Кт20 отклонения (операция 4): productId -> sumDelta (Count==0)
    Dictionary<string, decimal> GetAllocDeltas43_20(DateTime from, DateTime to, string acc43Id, string acc20Id);

    // Дт90 Кт43 по плану (списание себестоимости при продаже): productId -> (qtySold, sumPlanCogs)
    Dictionary<string, (int qty, decimal sum)> GetSalesCogs90_43_Plan(DateTime from, DateTime to, string acc90Id, string acc43Id);

    // Дебетовый оборот 20 за период
    decimal GetDebitTurnover20(DateTime from, DateTime to, string acc20Id);
}
