using System.Globalization;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;
using Contracts.ViewModels;

namespace Contracts.AdapterContracts;

public interface IDepartamentAdapterContract
{
    DepartamentOperationResponse GetList();

    DepartamentOperationResponse GetElement(string id);
    DepartamentOperationResponse GetDepartamentByName(string name);
    DepartamentOperationResponse GetDepartamentProductionListById(string id);
    DepartamentOperationResponse CreateDepartament(DepartamentBM departament);

    DepartamentOperationResponse UpdateDepartament(DepartamentBM departament);
    DepartamentOperationResponse RecoveryDepartament(string id);

    DepartamentOperationResponse DeleteDepartament(string id);
}
