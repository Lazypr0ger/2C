using Contracts.DTO;

namespace Contracts.Interfaces.Storages;

public interface ITransactionLogStorageContract
{
    List<TransactionLogDto> GetAll();
    List<TransactionLogDto> GetAllByDate(DateTime startDate, DateTime endDate);
    TransactionLogDto GetById(int id);

    void Create(TransactionLogDto transactionLogDto);
    void Update(TransactionLogDto transactionLogDto);
    void Delete(int id);
}
