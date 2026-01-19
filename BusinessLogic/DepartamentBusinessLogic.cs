using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class DepartamentBusinessLogic(
    IDepartamentStorageContract departamentStorageContract,
    ILogger<DepartamentBusinessLogic> logger) : IDepartamentBusinessLogic
{
    public void Create(DepartamentDto departamentsDto)
    {
        if (departamentsDto is null)
            throw new ArgumentNullException(nameof(departamentsDto));

        if (string.IsNullOrWhiteSpace(departamentsDto.Name))
            throw new ValidationException("Departament name is empty");


        if (string.IsNullOrWhiteSpace(departamentsDto.Id))
            departamentsDto.Id = Guid.NewGuid().ToString();


        departamentsDto.IsDeleted = false;

        logger.LogInformation("Creating departament. Name={Name}, Id={Id}", departamentsDto.Name, departamentsDto.Id);

        departamentStorageContract.Create(departamentsDto);
    }

    public void Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Departament id is empty");

        logger.LogInformation("Deleting departament. Id={Id}", id);
        departamentStorageContract.Delete(id);
    }

    public List<DepartamentDto> GetAll()
    {
        var list = departamentStorageContract.GetAll();
        return list ?? throw new NullListException();
    }

    public DepartamentDto GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Departament id is empty");

        return departamentStorageContract.GetById(id)
               ?? throw new ElementNotFoundException($"Element with {id} not found");
    }

    public DepartamentDto GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Departament name is empty");

        return departamentStorageContract.GetByName(name)
               ?? throw new ElementNotFoundException($"Element with {name} not found");
    }

    public void Recovery(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Departament id is empty");

        logger.LogInformation("Recovery departament. Id={Id}", id);
        departamentStorageContract.Recovery(id);
    }

    public void Update(DepartamentDto departamentsDto)
    {
        if (departamentsDto is null)
            throw new ArgumentNullException(nameof(departamentsDto));

        if (string.IsNullOrWhiteSpace(departamentsDto.Id))
            throw new ValidationException("Departament id is empty");

        if (string.IsNullOrWhiteSpace(departamentsDto.Name))
            throw new ValidationException("Departament name is empty");

        logger.LogInformation("Updating departament. Id={Id}", departamentsDto.Id);

        departamentStorageContract.Update(departamentsDto);
    }
}
