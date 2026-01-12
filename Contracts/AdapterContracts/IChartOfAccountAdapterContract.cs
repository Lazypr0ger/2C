
using Contracts.AdapterContracts.OperationResponses;
using Contracts.ViewModels;

namespace Contracts.AdapterContracts;

public interface IChartOfAccountAdapterContract
{
    ChartOfAccountOperationResponse GetList();

    ChartOfAccountOperationResponse GetElement(string id);
    ChartOfAccountOperationResponse GetChartByName(string name);
    ChartOfAccountOperationResponse GetChartByNum(string num);
    ChartOfAccountOperationResponse CreateChart(ChartOfAccountVM chrtmodel);
}

