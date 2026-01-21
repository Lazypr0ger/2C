using Contracts.BindingModels;
using Contracts.DTO.Reports;
using Contracts.Enums;
using Contracts.ViewModels.Reports;
using static _2Cclient.Views.Pages.Reports.ReportsListPage;

namespace _2Cclient.Services.Api;

public class ReportApi
{
    private readonly ApiClient _api;
    public ReportApi(ApiClient api) => _api = api;

    public Task<ReportResultVM> BuildAsync(ReportBuildBM bm, CancellationToken ct = default)
        => _api.PostAsync<ReportResultVM, ReportBuildBM>("/ms/api/Report/build", bm, ct);

    public Task<List<ReportListItemVM>> GetListAsync(ReportTypeCodes? typeCode = null, CancellationToken ct = default)
    {
        var url = typeCode.HasValue
            ? $"/ms/api/Report/list?typeCode={Uri.EscapeDataString(typeCode.Value.ToString())}"
            : "/ms/api/Report/list";

        return _api.GetAsync<List<ReportListItemVM>>(url, ct);
    }
    public Task DeleteAsync(string id, CancellationToken ct = default)
    => _api.DeleteAsync($"/ms/api/Report/{id}", ct);

}
