using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class OrganisationBusinessLogic(
    IOrganisationStorageContract organisationStorage,
    ILogger<OrganisationBusinessLogic> logger) : IOrganisationBusinessLogic
{
    public void Create(OrganisationDto organisationDto)
    {
        if (organisationDto is null)
            throw new ArgumentNullException(nameof(organisationDto));

        if (string.IsNullOrWhiteSpace(organisationDto.Name))
            throw new ValidationException("Organisation name is empty");

        if (string.IsNullOrWhiteSpace(organisationDto.AccountNumOrg))
            throw new ValidationException("Organisation account number is empty");

        // ✅ Id генерируется в BL
        if (string.IsNullOrWhiteSpace(organisationDto.Id))
            organisationDto.Id = Guid.NewGuid().ToString();

        // ✅ при создании всегда активная
        organisationDto.IsDeleted = false;

        logger.LogInformation("Creating organisation. Name={Name}, Account={Account}, Id={Id}",
            organisationDto.Name, organisationDto.AccountNumOrg, organisationDto.Id);

        organisationStorage.Create(organisationDto);
    }

    public void Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Organisation id is empty");

        logger.LogInformation("Deleting organisation. Id={Id}", id);
        organisationStorage.Delete(id);
    }

    public List<OrganisationDto> GetAll()
    {
        var list = organisationStorage.GetAll();
        return list ?? throw new NullListException();
    }

    public OrganisationDto GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Organisation id is empty");

        return organisationStorage.GetById(id)
               ?? throw new ElementNotFoundException($"Организация с Id {id} не найдена");
    }

    public OrganisationDto GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Organisation name is empty");

        return organisationStorage.GetByName(name)
               ?? throw new ElementNotFoundException($"Организация с именем {name} не найдена");
    }

    public void Recovery(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Organisation id is empty");

        logger.LogInformation("Recovery organisation. Id={Id}", id);
        organisationStorage.Recovery(id);
    }

    public void Update(OrganisationDto organisationDto)
    {
        if (organisationDto is null)
            throw new ArgumentNullException(nameof(organisationDto));

        if (string.IsNullOrWhiteSpace(organisationDto.Id))
            throw new ValidationException("Organisation id is empty");

        if (string.IsNullOrWhiteSpace(organisationDto.Name))
            throw new ValidationException("Organisation name is empty");

        if (string.IsNullOrWhiteSpace(organisationDto.AccountNumOrg))
            throw new ValidationException("Organisation account number is empty");

        logger.LogInformation("Updating organisation. Id={Id}", organisationDto.Id);
        organisationStorage.Update(organisationDto);
    }
}
