using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;

namespace Contracts.AdapterContracts;

public interface IElementAdapterContract
{
    ElementOperationResponse GetList();
    ElementOperationResponse GetElement(string id);
    ElementOperationResponse GetByOperationId(string operationId);

    ElementOperationResponse Create(ElementBM bm);
    ElementOperationResponse Update(ElementBM bm);

    ElementOperationResponse Recovery(string id);
    ElementOperationResponse Delete(string id);
}
