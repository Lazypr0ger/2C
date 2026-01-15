using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class OrganisationBusinessLogic(IOrganisationStorageContract organisationStorage, ILogger<OrganisationBusinessLogic> logger) : IOrganisationBusinessLogic
{
    public void Create(OrganisationDto organisationDto)
    {
        organisationStorage.Create(organisationDto);
    }

    public void Delete(string id)
    {
        organisationStorage.Delete(id);
    }

    public List<OrganisationDto> GetAll()
    {
        return organisationStorage.GetAll() ?? throw new NullListException();
    }

    public OrganisationDto GetById(string id)
    {
        return organisationStorage.GetById(id) ?? throw new ElementNotFoundException($"Организация с Id {id} не найдена");
    }

    public OrganisationDto GetByName(string name)
    {
        return organisationStorage.GetByName(name) ?? throw new ElementNotFoundException($"Организация с именем {name} не найдена");
    }

    public void Recovery(string id)
    {
        organisationStorage.Recovery(id);
    }

    public void Update(OrganisationDto organisationDto)
    {
        organisationStorage.Update(organisationDto);
    }
}
