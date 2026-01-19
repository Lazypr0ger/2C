
using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;
using Contracts.ViewModels;

namespace Contracts.AdapterContracts;

public interface IChartOfAccountAdapterContract
{
    ChartOfAccountOperationResponse GetList();

    ChartOfAccountOperationResponse GetElement(string id);
    ChartOfAccountOperationResponse GetChartByName(string name);
    ChartOfAccountOperationResponse GetChartByNum(string num);
    ChartOfAccountOperationResponse CreateChart(ChartOfAccountBM chrtmodel);
}

