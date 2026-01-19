using Contracts.ViewModels;

namespace Contracts.AdapterContracts.OperationResponses;

public class TransactionLogOperationResponse : OperationResponse
{
    public static TransactionLogOperationResponse OK(List<TransactionLogVM> data)
        => OK<TransactionLogOperationResponse, List<TransactionLogVM>>(data);

    public static TransactionLogOperationResponse OK(TransactionLogVM data)
        => OK<TransactionLogOperationResponse, TransactionLogVM>(data);

    public static TransactionLogOperationResponse NoContent()
        => NoContent<TransactionLogOperationResponse>();

    public static TransactionLogOperationResponse BadRequest(string msg)
        => BadRequest<TransactionLogOperationResponse>(msg);

    public static TransactionLogOperationResponse NotFound(string msg)
        => NotFound<TransactionLogOperationResponse>(msg);

    public static TransactionLogOperationResponse InternalServerError(string msg)
        => InternalServerError<TransactionLogOperationResponse>(msg);
}
