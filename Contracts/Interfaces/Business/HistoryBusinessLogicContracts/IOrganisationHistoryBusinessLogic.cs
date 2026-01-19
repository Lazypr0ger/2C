using Contracts.DTO;
using Contracts.DTO.HistoriesDto;

namespace Contracts.Interfaces.Business.HistoryBusinessLogicContracts;

public interface IOrganisationHistoryBusinessLogic
{
    List<OrganisationHistoryDto> GetHistory(string organisationId);
    OrganisationDto GetAsOf(string organisationId, DateTime atUtc);
    void RestoreFromHistory(string historyId);
}
