using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class OrganisationBusinessLogic(IOrganisationStorageContract storage, ILogger<OrganisationBusinessLogic> logger) : IOrganisationBusinessLogic
{
    public void Create(OrganisationDto organisationDto)
    {
        storage.Create(organisationDto);
    }

    public void Delete(string id)
    {
        storage.Delete(id);
    }

    public List<OrganisationDto> GetAll()
    {
        return storage.GetAll() ?? throw new NullListException();
    }

    public OrganisationDto GetById(string id)
    {
        return storage.GetById(id) ?? throw new ElementNotFoundException($"Организация с Id {id} не найдена");
    }

    public OrganisationDto GetByName(string name)
    {
        return storage.GetByName(name) ?? throw new ElementNotFoundException($"Организация с именем {name} не найдена");
    }

    public void Update(OrganisationDto organisationDto)
    {
        storage.Update(organisationDto);
    }
}
