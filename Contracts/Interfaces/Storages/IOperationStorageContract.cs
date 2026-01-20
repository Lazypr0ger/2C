using Contracts.DTO;

namespace Contracts.Interfaces.Storages;

public interface IOperationStorageContract
{
    List<OperationDto> GetAll(DateTime? from = null, DateTime? to = null);
    OperationDto? GetById(string id);

    void Delete(string id);
    void Recovery(string id);

    Dictionary<string, string> GetAccountIdsByNums(IEnumerable<string> nums);
    Dictionary<string, decimal> GetPlannedCostsByProductIds(IEnumerable<string> productIds);

    Dictionary<string, (int qty, decimal sum)> GetReceipts43_20_Plan(DateTime from, DateTime to, string acc43Id, string acc20Id);
    Dictionary<string, decimal> GetAllocDeltas43_20(DateTime from, DateTime to, string acc43Id, string acc20Id);
    Dictionary<string, (int qty, decimal sum)> GetSalesCogs90_43_Plan(DateTime from, DateTime to, string acc90Id, string acc43Id);
    decimal GetDebitTurnover20(DateTime from, DateTime to, string acc20Id);

    // НОВОЕ: атомарное создание/обновление документа
    void CreateDocument(OperationDto dto, List<ElementDto> elements, List<TransactionLogDto> logs);
    void UpdateDocument(OperationDto dto, List<ElementDto> elements, List<TransactionLogDto> logs);
}
