using Contracts.ViewModels.HistoryModels;

namespace _2Cclient.Services.Api.HistoryApi
{
    public class OrganisationHistoryApi
    {
        private readonly ApiClient _api;

        public OrganisationHistoryApi(ApiClient api) => _api = api;

        public Task<List<OrganisationHistoryVM>> GetHistoryAsync(string organisationId, CancellationToken ct = default)
            => _api.GetAsync<List<OrganisationHistoryVM>>($"/ms/api/OrganisationHistory/{organisationId}", ct);

        public Task RestoreFromHistoryAsync(string historyId, CancellationToken ct = default)
            => _api.PatchAsync($"/ms/api/OrganisationHistory/restore/{historyId}", ct);
    }
}
