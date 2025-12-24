using Contracts.DTO;
using Contracts.Interfaces.Storages;


namespace DataBase.Implementation;

public class OrganisationStorageContract : IOrganisationStorageContract
{
    public void Create(OrganisationDto organisationDto)
    {
        throw new NotImplementedException();
    }

    public void Delete(string id)
    {
        throw new NotImplementedException();
    }

    public List<OrganisationDto> GetAll()
    {
        throw new NotImplementedException();
    }

    public OrganisationDto GetById(int id)
    {
        throw new NotImplementedException();
    }

    public OrganisationDto GetByName(string name)
    {
        throw new NotImplementedException();
    }

    public void Update(OrganisationDto organisationDto)
    {
        throw new NotImplementedException();
    }
}
