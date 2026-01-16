using Contracts.Infrastructure;
using Contracts.ViewModels;

namespace Contracts.AdapterContracts.OperationResponses;

public class ProductionOperationResponse : OperationResponse
{
    public static ProductionOperationResponse OK(List<ProductionVM> data) => OK<ProductionOperationResponse, List<ProductionVM>>(data);

    public static ProductionOperationResponse OK(ProductionVM data) => OK<ProductionOperationResponse, ProductionVM>(data);

    public static ProductionOperationResponse NoContent() => NoContent<ProductionOperationResponse>();

    public static ProductionOperationResponse BadRequest(string message) => BadRequest<ProductionOperationResponse>(message);

    public static ProductionOperationResponse NotFound(string message) => NotFound<ProductionOperationResponse>(message);

    public static ProductionOperationResponse InternalServerError(string message) => InternalServerError<ProductionOperationResponse>(message);
}
