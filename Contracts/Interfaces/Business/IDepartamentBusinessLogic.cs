using Contracts.DTO;

namespace Contracts.Interfaces.Business;

public interface IDepartamentBusinessLogic
{
    List<DepartamentDto> GetAll();
    DepartamentDto GetById(string id);
    DepartamentDto GetByName(string name);

    List<DepartamentDto> GetDepartamentsByChart(string chartnum);

    void Create(DepartamentDto departamentsDto);
    void Update(DepartamentDto departamentsDto);
    void Delete(string id);


}
