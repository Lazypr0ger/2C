using Contracts.DTO;

namespace Contracts.Interfaces.Business;

public interface ITransactionLogBusinessLogic
{
    List<TransactionLogDto> GetAll(DateTime? from = null, DateTime? to = null);
    TransactionLogDto GetById(string id);

    List<TransactionLogDto> GetByOperationId(string operationId);

    void Create(TransactionLogDto dto);
    void Update(TransactionLogDto dto);

    void Delete(string id);
    void Recovery(string id);
}
