using Contracts.DTO;

namespace Contracts.Interfaces.Business;

public interface IChartOfAccountBusinessLogic
{
    List<ChartOfAccountDto> GetAll();
    ChartOfAccountDto GetById(int id);
    ChartOfAccountDto GetByNumChart(string NumChart);
    ChartOfAccountDto GetByNameChart(string NameChart);

    void Create(ChartOfAccountDto hartOfAccountDto);
    void Update(ChartOfAccountDto hartOfAccountDto);
    void Delete(string id);
}
