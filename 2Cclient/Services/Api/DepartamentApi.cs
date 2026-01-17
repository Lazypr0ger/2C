using Contracts.ViewModels;

namespace _2Cclient.Services.Api
{
    public class DepartamentApi
    {
        private readonly ApiClient _api;

        public DepartamentApi(ApiClient api) => _api = api;

        public Task<List<DepartamentVM>> GetAllAsync(CancellationToken ct = default)
            => _api.GetAsync<List<DepartamentVM>>("/ms/api/Departament", ct);

        public Task CreateAsync(DepartamentVM vm, CancellationToken ct = default)
            => _api.PostAsync("/ms/api/Departament", vm, ct);

        public Task UpdateAsync(DepartamentVM vm, CancellationToken ct = default)
            => _api.PutAsync("/ms/api/Departament", vm, ct);

        // “Удалить” = пометить IsDeleted=true (через PATCH)
        public Task SoftDeleteAsync(string id, CancellationToken ct = default)
            => _api.PatchAsync($"/ms/api/Departament/{id}", new { IsDeleted = true }, ct);

        // “Восстановить” = IsDeleted=false (через PATCH)
        public Task RestoreAsync(string id, CancellationToken ct = default)
            => _api.PatchAsync($"/ms/api/Departament/{id}", new { IsDeleted = false }, ct);
    }
}
