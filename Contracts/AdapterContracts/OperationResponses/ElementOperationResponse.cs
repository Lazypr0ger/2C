using Contracts.ViewModels;

namespace Contracts.AdapterContracts.OperationResponses;

public class ElementOperationResponse : OperationResponse
{
    public static ElementOperationResponse OK(List<ElementVM> data)
        => OK<ElementOperationResponse, List<ElementVM>>(data);

    public static ElementOperationResponse OK(ElementVM data)
        => OK<ElementOperationResponse, ElementVM>(data);

    public static ElementOperationResponse NoContent()
        => NoContent<ElementOperationResponse>();

    public static ElementOperationResponse BadRequest(string msg)
        => BadRequest<ElementOperationResponse>(msg);

    public static ElementOperationResponse NotFound(string msg)
        => NotFound<ElementOperationResponse>(msg);

    public static ElementOperationResponse InternalServerError(string msg)
        => InternalServerError<ElementOperationResponse>(msg);
}
