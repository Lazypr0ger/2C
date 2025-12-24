using Contracts.DTO;
using Contracts.Interfaces.Storages;

namespace DataBase.Implementation;

public class ChartOfAccountStorageContract : IChartOfAccountStorageContract
{
    public void Create(ChartOfAccountDto hartOfAccountDto)
    {
        throw new NotImplementedException();
    }

    public void Delete(string id)
    {
        throw new NotImplementedException();
    }

    public List<ChartOfAccountDto> GetAll()
    {
        throw new NotImplementedException();
    }

    public ChartOfAccountDto GetById(int id)
    {
        throw new NotImplementedException();
    }

    public ChartOfAccountDto GetByNameChart(string NameChart)
    {
        throw new NotImplementedException();
    }

    public ChartOfAccountDto GetByNumChart(string NumChart)
    {
        throw new NotImplementedException();
    }

    public void Update(ChartOfAccountDto hartOfAccountDto)
    {
        throw new NotImplementedException();
    }
}
