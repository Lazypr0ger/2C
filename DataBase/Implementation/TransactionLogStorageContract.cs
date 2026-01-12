using Contracts.DTO;
using Contracts.Interfaces.Storages;


namespace DataBase.Implementation;

public class TransactionLogStorageContract : ITransactionLogStorageContract
{
    public void Create(TransactionLogDto transactionLogDto)
    {
        throw new NotImplementedException();
    }

    public void Delete(string id)
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

    public TransactionLogDto GetById(string id)
    {
        throw new NotImplementedException();
    }

    public void Update(TransactionLogDto transactionLogDto)
    {
        throw new NotImplementedException();
    }
}
