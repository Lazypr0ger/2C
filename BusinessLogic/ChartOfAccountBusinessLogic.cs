using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class ChartOfAccountBusinessLogic(IChartOfAccountStorageContract chartOfAccountStorageContract) : IChartOfAccountBusinessLogic
{
    private IChartOfAccountStorageContract? _chartOfAccount = chartOfAccountStorageContract;

    public void Create(ChartOfAccountDto chartOfAccountDto)
    {
        _chartOfAccount.Create(chartOfAccountDto);
    }

    public void Delete(string id)
    {
        throw new NotImplementedException();
    }

    public List<ChartOfAccountDto> GetAll()
    {
        return _chartOfAccount.GetAll() ?? throw new NullListException();
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
