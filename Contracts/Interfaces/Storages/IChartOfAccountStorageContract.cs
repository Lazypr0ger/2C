using Contracts.DTO;

namespace Contracts.Interfaces.Storages;

public interface IChartOfAccountStorageContract
{
    List<ChartOfAccountDto> GetAll();
    ChartOfAccountDto GetById(string id);
    ChartOfAccountDto GetByNumChart(string NumChart);
    ChartOfAccountDto GetByNameChart(string NameChart);

    void Create(ChartOfAccountDto chartOfAccountDto);
}
