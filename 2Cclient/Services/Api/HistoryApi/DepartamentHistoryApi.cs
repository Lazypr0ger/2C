using Contracts.ViewModels.HistoryModels;

namespace _2Cclient.Services.Api.HistoryApi;

public class DepartamentHistoryApi
{
    private readonly ApiClient _api;
    public DepartamentHistoryApi(ApiClient api) => _api = api;

    public Task<List<DepartamentHistoryVM>> GetHistoryAsync(string departamentId, CancellationToken ct = default)
        => _api.GetAsync<List<DepartamentHistoryVM>>($"/ms/api/DepartamentHistory/{departamentId}", ct);
    public Task RestoreFromHistoryAsync(string historyId, CancellationToken ct = default)
           => _api.PatchAsync($"/ms/api/DepartamentHistory/restore/{historyId}", ct);
}
