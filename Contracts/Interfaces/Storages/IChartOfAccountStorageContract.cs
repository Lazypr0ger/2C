using Contracts.DTO;

namespace Contracts.Interfaces.Storages;

public interface IChartOfAccountStorageContract
{
    List<ChartOfAccountDto> GetAll();
    ChartOfAccountDto GetById(int id);
    ChartOfAccountDto GetByNumChart(string NumChart);
    ChartOfAccountDto GetByNameChart(string NameChart);

    void Create(ChartOfAccountDto chartOfAccountDto);
    void Update(ChartOfAccountDto chartOfAccountDto);
    void Delete(string id);
}
