using Contracts.DTO;

namespace Contracts.Interfaces.Storages;

public interface ITransactionLogStorageContract
{
    List<TransactionLogDto> GetAll(DateTime? from = null, DateTime? to = null);
    TransactionLogDto GetById(string id);

    List<TransactionLogDto> GetByOperationId(string operationId);

    void Create(TransactionLogDto dto);
    void Update(TransactionLogDto dto);

    void Delete(string id);
    void Recovery(string id);
}
