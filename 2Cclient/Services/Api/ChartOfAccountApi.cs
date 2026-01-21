using Contracts.ViewModels;

namespace _2Cclient.Services.Api
{
    public class ChartOfAccountApi
    {
        private readonly ApiClient _api;

        public ChartOfAccountApi(ApiClient api) => _api = api;

        public Task<List<ChartOfAccountVM>> GetAllAsync(CancellationToken ct = default)
            => _api.GetAsync<List<ChartOfAccountVM>>("/ms/api/ChartOfAccount", ct);

        public Task<ChartOfAccountVM> GetByIdAsync(string id, CancellationToken ct = default)
            => _api.GetAsync<ChartOfAccountVM>($"/ms/api/ChartOfAccount/id/{id}", ct);

        public Task<List<ChartOfAccountVM>> GetByNameAsync(string name, CancellationToken ct = default)
            => _api.GetAsync<List<ChartOfAccountVM>>($"/ms/api/ChartOfAccount/name/{name}", ct);

        public Task<List<ChartOfAccountVM>> GetByNumAsync(string numChart, CancellationToken ct = default)
            => _api.GetAsync<List<ChartOfAccountVM>>($"/ms/api/ChartOfAccount/num/{numChart}", ct);
    }
}
