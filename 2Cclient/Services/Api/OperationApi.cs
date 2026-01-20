using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contracts.ViewModels;

namespace _2Cclient.Services.Api
{
    public class OperationApi
    {
        private readonly ApiClient _api;
        public OperationApi(ApiClient api) => _api = api;

        public Task<List<OperationVM>> GetAllAsync(DateTime? fromUtc, DateTime? toUtc, CancellationToken ct = default)
        {
            var q = "";
            if (fromUtc.HasValue) q += (q.Length == 0 ? "?" : "&") + $"from={Uri.EscapeDataString(fromUtc.Value.ToString("O"))}";
            if (toUtc.HasValue) q += (q.Length == 0 ? "?" : "&") + $"to={Uri.EscapeDataString(toUtc.Value.ToString("O"))}";

            return _api.GetAsync<List<OperationVM>>($"/ms/api/Operation{q}", ct);
        }

        public Task<OperationVM> GetByIdAsync(string id, CancellationToken ct = default)
            => _api.GetAsync<OperationVM>($"/ms/api/Operation/id/{id}", ct);

        public Task DeleteAsync(string id, CancellationToken ct = default)
            => _api.DeleteAsync($"/ms/api/Operation/{id}", new { }, ct);

        public Task RecoveryAsync(string id, CancellationToken ct = default)
            => _api.PatchAsync($"/ms/api/Operation/{id}", new { }, ct);
    }
}
