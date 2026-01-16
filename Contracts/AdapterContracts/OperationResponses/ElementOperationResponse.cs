using Contracts.Infrastructure;
using Contracts.ViewModels;

namespace Contracts.AdapterContracts.OperationResponses;

public class ElementOperationResponse : OperationResponse
{
    public static ElementOperationResponse OK(List<ElementVM> data) => OK<ElementOperationResponse, List<ElementVM>>(data);

    public static ElementOperationResponse OK(ElementVM data) => OK<ElementOperationResponse, ElementVM>(data);

    public static ElementOperationResponse OK(decimal total) => OK<ElementOperationResponse, decimal>(total);

    public static ElementOperationResponse NoContent() => NoContent<ElementOperationResponse>();

    public static ElementOperationResponse BadRequest(string message) => BadRequest<ElementOperationResponse>(message);

    public static ElementOperationResponse NotFound(string message) => NotFound<ElementOperationResponse>(message);

    public static ElementOperationResponse InternalServerError(string message) => InternalServerError<ElementOperationResponse>(message);
}
