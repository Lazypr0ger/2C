using Contracts.DTO;

namespace Contracts.Interfaces.Business;

public interface IDepartamentBusinessLogic
{
    List<DepartamentsDto> GetAll();
    DepartamentsDto GetById(int id);
    DepartamentsDto GetByName(string name);

    void Create(DepartamentsDto departamentsDto);
    void Update(DepartamentsDto departamentsDto);
    void Delete(int id);


}
