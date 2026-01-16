using Contracts.AdapterContracts.OperationResponses;
using Contracts.ViewModels;

namespace Contracts.AdapterContracts;

public interface IElementAdapterContract
{
    ElementOperationResponse GetAllElement();
    ElementOperationResponse GetElementById(string elementId);
    ElementOperationResponse CreateElement(ElementVM element);
    ElementOperationResponse UpdateElement(ElementVM element);
    ElementOperationResponse DeleteElement(string elementId);
    ElementOperationResponse RecoveryElement(string elementId);

    ElementOperationResponse CalculateTotalCostElement(int countelement, decimal realisationCost);
}
