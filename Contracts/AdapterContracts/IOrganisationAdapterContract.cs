using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;
using Contracts.DTO;
using Contracts.ViewModels;

namespace Contracts.AdapterContracts;

public interface IOrganisationAdapterContract
{
    OrganisationOperationResponse GetAll();
    OrganisationOperationResponse GetById(string id);
    OrganisationOperationResponse GetByName(string name);
    OrganisationOperationResponse Create(OrganisationBM organisation);
    OrganisationOperationResponse Update(OrganisationBM organisation);
    OrganisationOperationResponse RecoveryOrganisation(string id);
    OrganisationOperationResponse Delete(string id);
}
