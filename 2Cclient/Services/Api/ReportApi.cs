using Contracts.BindingModels;
using Contracts.ViewModels.Reports;

namespace _2Cclient.Services.Api;

public class ReportApi
{
    private readonly ApiClient _api;
    public ReportApi(ApiClient api) => _api = api;

    public Task<ReportResultVM> BuildAsync(ReportBuildBM bm, CancellationToken ct = default)
        => _api.PostAsync<ReportResultVM, ReportBuildBM>("/ms/api/Report/build", bm, ct);
}
