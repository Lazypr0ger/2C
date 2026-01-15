using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class DepartamentBusinessLogic(IDepartamentStorageContract departamentStorageContract, ILogger<DepartamentBusinessLogic> logger) : IDepartamentBusinessLogic
{

    public void Create(DepartamentDto departamentsDto)
    {
        departamentStorageContract.Create(departamentsDto);
    }

    public void Delete(string id)
    {
        departamentStorageContract.Delete(id);
    }

    public List<DepartamentDto> GetAll()
    {
        return departamentStorageContract.GetAll() ?? throw new NullListException();
    }

    public DepartamentDto GetById(string id)
    {
        return departamentStorageContract.GetById(id) ?? throw new ElementNotFoundException("Element with "+ id +" not found");
    }

    public DepartamentDto GetByName(string name)
    {
        return departamentStorageContract.GetByName(name) ?? throw new ElementNotFoundException("Element with " + name + " not found");
    }

    public void Recovery(string id)
    {
        departamentStorageContract.Recovery(id);
    }

    public void Update(DepartamentDto departamentsDto)
    {
        departamentStorageContract.Update(departamentsDto);
    }

}
