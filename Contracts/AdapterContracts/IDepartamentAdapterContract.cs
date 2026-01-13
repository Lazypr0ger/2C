using System.Globalization;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.ViewModels;

namespace Contracts.AdapterContracts;

public interface IDepartamentAdapterContract
{
    DepartamentOperationResponse GetList();

    DepartamentOperationResponse GetElement(string id);
    DepartamentOperationResponse GetDepartamentByName(string name);
    DepartamentOperationResponse GetDepartamentListByChartNum(string num);

    DepartamentOperationResponse GetDepartamentProductionListById(string id);
    DepartamentOperationResponse CreateDepartament(DepartamentVM departament);

    DepartamentOperationResponse UpdateDepartament(DepartamentVM departament);

    DepartamentOperationResponse DeleteDepartament(string id);
}
