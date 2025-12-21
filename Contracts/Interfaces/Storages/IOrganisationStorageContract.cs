using Contracts.DTO;

namespace Contracts.Interfaces.Storages
{
    public interface IOrganisationStorageContract
    {
        List<OrganisationDto> GetAll();
        OrganisationDto GetById(int id);
        OrganisationDto GetByName(string name);
        void Create(OrganisationDto organisationDto);
        void Update(OrganisationDto organisationDto);
        void Delete(string id);
    }
}
