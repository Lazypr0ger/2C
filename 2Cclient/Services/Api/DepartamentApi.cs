using Contracts.BindingModels;
using Contracts.ViewModels;

namespace _2Cclient.Services.Api
{
    public class DepartamentApi
    {
        private readonly ApiClient _api;

        public DepartamentApi(ApiClient api) => _api = api;

        public Task<List<DepartamentVM>> GetAllAsync(CancellationToken ct = default)
            => _api.GetAsync<List<DepartamentVM>>("/ms/api/Departament", ct);

        public Task CreateAsync(string name, CancellationToken ct = default)
            => _api.PostAsync("/ms/api/Departament", new { Name = name }, ct);


        public Task CreateAsync(DepartamentBM vm, CancellationToken ct = default)
            => _api.PostAsync("/ms/api/Departament", vm, ct);

        public Task UpdateAsync(DepartamentBM vm, CancellationToken ct = default)
            => _api.PutAsync("/ms/api/Departament", vm, ct);

        public Task SoftDeleteAsync(string id, CancellationToken ct = default)
            => _api.DeleteAsync($"/ms/api/Departament/{id}", ct);

        public Task RestoreAsync(string id, CancellationToken ct = default)
            => _api.PatchAsync($"/ms/api/Departament/{id}", ct);

    }
}
