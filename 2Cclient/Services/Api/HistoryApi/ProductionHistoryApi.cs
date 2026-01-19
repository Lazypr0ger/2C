using Contracts.ViewModels.HistoryModels;

namespace _2Cclient.Services.Api.HistoryApi
{
    public class ProductionHistoryApi
    {
        private readonly ApiClient _api;

        public ProductionHistoryApi(ApiClient api) => _api = api;

        public Task<List<ProductionHistoryVM>> GetHistoryAsync(string productionId, CancellationToken ct = default)
            => _api.GetAsync<List<ProductionHistoryVM>>($"/ms/api/ProductionHistory/{productionId}", ct);

        public Task RestoreFromHistoryAsync(string historyId, CancellationToken ct = default)
            => _api.PatchAsync($"/ms/api/ProductionHistory/restore/{historyId}", ct);
    }
}
