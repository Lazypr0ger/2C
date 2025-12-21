using Contracts.DTO;

namespace Contracts.Interfaces.Storages;

public interface IDepartamentStorageCointract
{
    List<DepartamentsDto> GetAll();
    DepartamentsDto GetById(int id);
    DepartamentsDto GetByName(string name);

    void Create(DepartamentsDto departamentsDto);
    void Update(DepartamentsDto departamentsDto);
    void Delete(int id);
}
