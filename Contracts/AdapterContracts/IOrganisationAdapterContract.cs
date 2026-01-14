using Contracts.AdapterContracts.OperationResponses;
using Contracts.DTO;
using Contracts.ViewModels;

namespace Contracts.AdapterContracts;

public interface IOrganisationAdapterContract
{
    OrganisationOperationResponse GetAll();
    OrganisationOperationResponse GetById(string id);
    OrganisationOperationResponse GetByName(string name);
    OrganisationOperationResponse Create(OrganisationVM organisation);
    OrganisationOperationResponse Update(OrganisationVM organisation);
    OrganisationOperationResponse Delete(string id);
}
