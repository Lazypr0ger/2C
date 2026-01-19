using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;

namespace Contracts.AdapterContracts;

public interface IOperationAdapterContract
{
    OperationOperationResponse GetList(DateTime? from = null, DateTime? to = null);
    OperationOperationResponse GetElement(string id);

    OperationOperationResponse Create(OperationBM bm);
    OperationOperationResponse Update(OperationBM bm);

    OperationOperationResponse Delete(string id);
    OperationOperationResponse Recovery(string id);
}
