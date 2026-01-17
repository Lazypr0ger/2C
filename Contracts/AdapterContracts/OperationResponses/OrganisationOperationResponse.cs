using Contracts.ViewModels;

namespace Contracts.AdapterContracts.OperationResponses;

public class OrganisationOperationResponse : OperationResponse
{
    public static OrganisationOperationResponse OK(List<OrganisationVM> data) => OK<OrganisationOperationResponse, List<OrganisationVM>>(data);

    public static OrganisationOperationResponse OK(OrganisationVM data) => OK<OrganisationOperationResponse, OrganisationVM>(data);

    public static OrganisationOperationResponse NoContent() => NoContent<OrganisationOperationResponse>();

    public static OrganisationOperationResponse BadRequest(string message) => BadRequest<OrganisationOperationResponse>(message);

    public static OrganisationOperationResponse NotFound(string message) => NotFound<OrganisationOperationResponse>(message);

    public static OrganisationOperationResponse InternalServerError(string message) => InternalServerError<OrganisationOperationResponse>(message);
}
