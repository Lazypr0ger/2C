using Contracts.ViewModels;
namespace Contracts.AdapterContracts.OperationResponses;

public class ChartOfAccountOperationResponse : OperationResponse
{
    public static ChartOfAccountOperationResponse OK(List<ChartOfAccountVM> data) => OK<ChartOfAccountOperationResponse, List<ChartOfAccountVM>>(data);

    public static ChartOfAccountOperationResponse OK(ChartOfAccountVM data) => OK<ChartOfAccountOperationResponse, ChartOfAccountVM>(data);

    public static ChartOfAccountOperationResponse NoContent() => NoContent<ChartOfAccountOperationResponse>();

    public static ChartOfAccountOperationResponse BadRequest(string message) => BadRequest<ChartOfAccountOperationResponse>(message);

    public static ChartOfAccountOperationResponse NotFound(string message) => NotFound<ChartOfAccountOperationResponse>(message);

    public static ChartOfAccountOperationResponse InternalServerError(string message) => InternalServerError<ChartOfAccountOperationResponse>(message);
}
