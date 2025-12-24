using Contracts.DTO;

namespace Contracts.Interfaces.Storages;

public interface IDepartamentStorageCointract
{
    List<DepartamentDto> GetAll();
    DepartamentDto GetById(int id);
    DepartamentDto GetByName(string name);

    void Create(DepartamentDto departamentsDto);
    void Update(DepartamentDto departamentsDto);
    void Delete(int id);
}
