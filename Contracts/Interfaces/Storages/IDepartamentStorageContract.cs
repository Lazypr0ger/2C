using Contracts.DTO;

namespace Contracts.Interfaces.Storages;

public interface IDepartamentStorageContract
{
    List<DepartamentDto> GetAll();
    DepartamentDto GetById(string id);
    DepartamentDto GetByName(string name);

    List<DepartamentDto> GetByChartNum(string chartNum);

    void Create(DepartamentDto departamentsDto);
    void Update(DepartamentDto departamentsDto);
    void Delete(string id);
}
