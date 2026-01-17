using Contracts.ViewModels;

namespace _2Cclient.Services.Api
{
    public class ProductionApi
    {
        private readonly ApiClient _api;
        public ProductionApi(ApiClient api) => _api = api;

        public Task<List<ProductionVM>> GetAllAsync(CancellationToken ct = default)
            => _api.GetAsync<List<ProductionVM>>("/ms/api/Production", ct);

        public Task CreateAsync(ProductionVM vm, CancellationToken ct = default)
            => _api.PostAsync("/ms/api/Production", vm, ct);

        public Task UpdateAsync(ProductionVM vm, CancellationToken ct = default)
            => _api.PutAsync("/ms/api/Production", vm, ct);

        public Task SoftDeleteAsync(string id, CancellationToken ct = default)
            => _api.PatchAsync($"/ms/api/Production/{id}", new { IsDeleted = true }, ct);

        public Task RestoreAsync(string id, CancellationToken ct = default)
            => _api.PatchAsync($"/ms/api/Production/{id}", new { IsDeleted = false }, ct);
    }
}
