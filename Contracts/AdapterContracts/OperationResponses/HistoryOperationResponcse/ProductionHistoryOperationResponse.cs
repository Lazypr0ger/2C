using Contracts.ViewModels;
using Contracts.ViewModels.HistoryModels;

namespace Contracts.AdapterContracts.OperationResponses.HistoryOperationResponcse;

public class ProductionHistoryOperationResponse : OperationResponse
{
    public static ProductionHistoryOperationResponse OK(List<ProductionHistoryVM> data)
        => OK<ProductionHistoryOperationResponse, List<ProductionHistoryVM>>(data);

    public static ProductionHistoryOperationResponse OK(ProductionVM data)
        => OK<ProductionHistoryOperationResponse, ProductionVM>(data);

    public static ProductionHistoryOperationResponse NoContent()
        => NoContent<ProductionHistoryOperationResponse>();

    public static ProductionHistoryOperationResponse BadRequest(string message)
        => BadRequest<ProductionHistoryOperationResponse>(message);

    public static ProductionHistoryOperationResponse NotFound(string message)
        => NotFound<ProductionHistoryOperationResponse>(message);

    public static ProductionHistoryOperationResponse InternalServerError(string message)
        => InternalServerError<ProductionHistoryOperationResponse>(message);
}
