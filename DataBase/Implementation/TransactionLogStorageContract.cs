using Contracts.DTO;
using Contracts.Interfaces.Storages;


namespace DataBase.Implementation;

public class TransactionLogStorageContract : ITransactionLogStorageContract
{
    public void Create(TransactionLogDto transactionLogDto)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public List<TransactionLogDto> GetAll()
    {
        throw new NotImplementedException();
    }

    public List<TransactionLogDto> GetAllByDate(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public TransactionLogDto GetById(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(TransactionLogDto transactionLogDto)
    {
        throw new NotImplementedException();
    }
}
