using Contracts.DTO;

namespace Contracts.Interfaces.Business;

public interface IDepartamentBusinessLogic
{
    List<DepartamentDto> GetAll();
    DepartamentDto GetById(string id);
    DepartamentDto GetByName(string name);

    void Create(DepartamentDto departamentsDto);
    void Update(DepartamentDto departamentsDto);
    void Recovery(string id);
    void Delete(string id);


}
