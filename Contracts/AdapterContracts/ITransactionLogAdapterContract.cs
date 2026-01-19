using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;

namespace Contracts.AdapterContracts;

public interface ITransactionLogAdapterContract
{
    TransactionLogOperationResponse GetList(DateTime? from = null, DateTime? to = null);
    TransactionLogOperationResponse GetView(DateTime? from = null, DateTime? to = null);
    TransactionLogOperationResponse GetElement(string id);

    TransactionLogOperationResponse GetByOperationId(string operationId);

    TransactionLogOperationResponse Create(TransactionLogBM bm);
    TransactionLogOperationResponse Update(TransactionLogBM bm);

    TransactionLogOperationResponse Delete(string id);
    TransactionLogOperationResponse Recovery(string id);
}
