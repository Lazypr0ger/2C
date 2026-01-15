using Contracts.DTO;

namespace Contracts.Interfaces.Business;

public interface IOrganisationBusinessLogic
{
    List<OrganisationDto> GetAll();
    OrganisationDto GetById(string id);
    OrganisationDto GetByName(string name);
    void Create(OrganisationDto organisationDto);
    void Update(OrganisationDto organisationDto);
    void Recovery(string id);
    void Delete(string id);
}
