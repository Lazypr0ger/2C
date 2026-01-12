using Contracts.DTO;

namespace Contracts.Interfaces.Storages;

public interface ITransactionLogStorageContract
{
    List<TransactionLogDto> GetAll();
    List<TransactionLogDto> GetAllByDate(DateTime startDate, DateTime endDate);
    TransactionLogDto GetById(string id);

    void Create(TransactionLogDto transactionLogDto);
    void Update(TransactionLogDto transactionLogDto);
    void Delete(string id);
}
