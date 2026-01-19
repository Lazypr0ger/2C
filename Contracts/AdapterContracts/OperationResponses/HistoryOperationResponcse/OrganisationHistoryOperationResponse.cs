using Contracts.ViewModels;
using Contracts.ViewModels.HistoryModels;

namespace Contracts.AdapterContracts.OperationResponses.HistoryOperationResponcse;

public class OrganisationHistoryOperationResponse : OperationResponse
{
    public static OrganisationHistoryOperationResponse OK(List<OrganisationHistoryVM> data)
        => OK<OrganisationHistoryOperationResponse, List<OrganisationHistoryVM>>(data);

    public static OrganisationHistoryOperationResponse OK(OrganisationVM data)
        => OK<OrganisationHistoryOperationResponse, OrganisationVM>(data);

    public static OrganisationHistoryOperationResponse NoContent()
        => NoContent<OrganisationHistoryOperationResponse>();

    public static OrganisationHistoryOperationResponse BadRequest(string message)
        => BadRequest<OrganisationHistoryOperationResponse>(message);

    public static OrganisationHistoryOperationResponse NotFound(string message)
        => NotFound<OrganisationHistoryOperationResponse>(message);

    public static OrganisationHistoryOperationResponse InternalServerError(string message)
        => InternalServerError<OrganisationHistoryOperationResponse>(message);
}
