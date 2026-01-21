using Contracts.ViewModels.Reports;

namespace Contracts.AdapterContracts.OperationResponses;

public class ReportOperationResponse : OperationResponse
{

    public static ReportOperationResponse OK(ReportResultVM data)
        => OK<ReportOperationResponse, ReportResultVM>(data);

    public static ReportOperationResponse NoContent()
        => NoContent<ReportOperationResponse>();

    public static ReportOperationResponse BadRequest(string msg)
        => BadRequest<ReportOperationResponse>(msg);

    public static ReportOperationResponse NotFound(string msg)
        => NotFound<ReportOperationResponse>(msg);

    public static ReportOperationResponse InternalServerError(string msg)
        => InternalServerError<ReportOperationResponse>(msg);
}
