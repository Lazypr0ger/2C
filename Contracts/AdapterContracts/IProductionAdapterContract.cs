using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;
using Contracts.Enums;
using Contracts.ViewModels;

namespace Contracts.AdapterContracts;

public interface IProductionAdapterContract
{
    ProductionOperationResponse GetAll();
    ProductionOperationResponse GetById(string id);
    ProductionOperationResponse GetByName(string name);
    ProductionOperationResponse GetByCode(string code);
    ProductionOperationResponse GetByType(TypeProduct product);
    ProductionOperationResponse GetProductsByDepartament(string departametnName);
    ProductionOperationResponse Create(ProductionBM production);
    ProductionOperationResponse Update(ProductionBM production);
    ProductionOperationResponse RecoveryProduct(string id);
    ProductionOperationResponse Delete(string id);
}
