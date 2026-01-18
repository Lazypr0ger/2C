using Contracts.ViewModels;

namespace _2Cclient.Services.Api
{
    public class OrganisationApi
    {
        private readonly ApiClient _api;
        public OrganisationApi(ApiClient api) => _api = api;

        public Task<List<OrganisationVM>> GetAllAsync(CancellationToken ct = default)
            => _api.GetAsync<List<OrganisationVM>>("/ms/api/Organisation", ct);

        // CREATE: отправляем только необходимые поля (без Id)
        public Task CreateAsync(string name, string accountNumOrg, CancellationToken ct = default)
            => _api.PostAsync("/ms/api/Organisation", new { Name = name, AccountNumOrg = accountNumOrg }, ct);

        // UPDATE: отправляем VM целиком
        public Task UpdateAsync(OrganisationVM vm, CancellationToken ct = default)
            => _api.PutAsync("/ms/api/Organisation", vm, ct);

        public Task SoftDeleteAsync(string id, CancellationToken ct = default)
            => _api.PatchAsync($"/ms/api/Organisation/{id}", new { IsDeleted = true }, ct);

        public Task RestoreAsync(string id, CancellationToken ct = default)
            => _api.PatchAsync($"/ms/api/Organisation/{id}", new { IsDeleted = false }, ct);
    }
}
