using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;

namespace Contracts.AdapterContracts;

public interface IReportAdapterContract
{
    ReportOperationResponse Build(ReportBuildBM bm);
    ReportOperationResponse Delete(string id);
}
