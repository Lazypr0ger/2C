using Contracts.DTO;

namespace Contracts.Interfaces.Business;

public interface IChartOfAccountBusinessLogic
{
    List<ChartOfAccountDto> GetAll();
    ChartOfAccountDto GetById(string id);
    ChartOfAccountDto GetByNumChart(string NumChart);
    ChartOfAccountDto GetByNameChart(string NameChart);

    void Create(ChartOfAccountDto hartOfAccountDto);
}
