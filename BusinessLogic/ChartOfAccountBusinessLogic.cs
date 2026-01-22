using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class ChartOfAccountBusinessLogic(IChartOfAccountStorageContract chartOfAccount) : IChartOfAccountBusinessLogic
{

    public void Create(ChartOfAccountDto chartOfAccountDto)
    {
        if (chartOfAccountDto is null)
            throw new ArgumentNullException(nameof(chartOfAccountDto));

        if (string.IsNullOrWhiteSpace(chartOfAccountDto.NumChart))
            throw new ValidationException("NumChart is empty");

        if (string.IsNullOrWhiteSpace(chartOfAccountDto.Name))
            throw new ValidationException("Name is empty");

        if (string.IsNullOrWhiteSpace(chartOfAccountDto.Id))
            chartOfAccountDto.Id = Guid.NewGuid().ToString(); 


        chartOfAccount.Create(chartOfAccountDto);
    }
    public List<ChartOfAccountDto> GetAll()
    {
        return chartOfAccount.GetAll() ?? throw new NullListException();
    }

    public ChartOfAccountDto GetById(string id)
    {
        return chartOfAccount.GetById(id) ?? throw new ElementNotFoundException("Element with "+id+" not found");
    }

    public ChartOfAccountDto GetByNameChart(string NameChart)
    {
        return chartOfAccount.GetByNameChart(NameChart) ?? throw new ElementNotFoundException("Element with " + NameChart + " not found");
    }

    public ChartOfAccountDto GetByNumChart(string NumChart)
    {
        return chartOfAccount.GetByNumChart(NumChart) ?? throw new ElementNotFoundException("Element with " + NumChart + " not found");
    }
}
