using Contracts.DTO;

namespace Contracts.Interfaces.Business;

public interface ITransactionLogBusinessLogic
{
    List<TransactionLogDto> GetAll();
    List<TransactionLogDto> GetAllByDate(DateTime startDate , DateTime endDate);
    TransactionLogDto GetById(string id);

    void Create(TransactionLogDto transactionLogDto);
    void Update(TransactionLogDto transactionLogDto);
    void Delete(string id);
}
