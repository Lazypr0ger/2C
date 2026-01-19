using Contracts.ViewModels;

namespace Contracts.AdapterContracts.OperationResponses;

public class OperationOperationResponse : OperationResponse
{
    public static OperationOperationResponse OK(List<OperationVM> data)
        => OK<OperationOperationResponse, List<OperationVM>>(data);

    public static OperationOperationResponse OK(OperationVM data)
        => OK<OperationOperationResponse, OperationVM>(data);

    public static OperationOperationResponse NoContent()
        => NoContent<OperationOperationResponse>();

    public static OperationOperationResponse BadRequest(string msg)
        => BadRequest<OperationOperationResponse>(msg);

    public static OperationOperationResponse NotFound(string msg)
        => NotFound<OperationOperationResponse>(msg);

    public static OperationOperationResponse InternalServerError(string msg)
        => InternalServerError<OperationOperationResponse>(msg);
}
