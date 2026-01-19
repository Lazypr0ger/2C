using Contracts.DTO;
using Contracts.DTO.HistoriesDto;

namespace Contracts.Interfaces.Storages.HistoryStorageContracts;

public interface IOrganisationHistoryStorageContract
{
    List<OrganisationHistoryDto> GetByOrganisationId(string organisationId);
    OrganisationDto GetAsOf(string organisationId, DateTime atUtc);
    void RestoreFromHistory(string historyId);
}
