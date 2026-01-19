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
}
