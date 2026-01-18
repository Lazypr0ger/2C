using Contracts.Enums;
using Contracts.ViewModels;

namespace _2Cclient.Services.Api
{
    public class ProductionApi
    {
        private readonly ApiClient _api;
        public ProductionApi(ApiClient api) => _api = api;

        public Task<List<ProductionVM>> GetAllAsync(CancellationToken ct = default)
            => _api.GetAsync<List<ProductionVM>>("/ms/api/Production", ct);

        // CREATE: без Id
        public Task CreateAsync(string code, TypeProduct type, string name, decimal plannedCost, string departamentId, CancellationToken ct = default)
            => _api.PostAsync("/ms/api/Production", new
            {
                Code = code,
                Type = type,
                Name = name,
                PlannedCost = plannedCost,
                DepartamentId = departamentId,
                IsDeleted = false
            }, ct);

        // UPDATE: VM целиком
        public Task UpdateAsync(ProductionVM vm, CancellationToken ct = default)
            => _api.PutAsync("/ms/api/Production", vm, ct);

        public Task SoftDeleteAsync(string id, CancellationToken ct = default)
            => _api.PatchAsync($"/ms/api/Production/{id}", new { IsDeleted = true }, ct);

        public Task RestoreAsync(string id, CancellationToken ct = default)
            => _api.PatchAsync($"/ms/api/Production/{id}", new { IsDeleted = false }, ct);
    }
}
