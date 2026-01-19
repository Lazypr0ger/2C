using Contracts.DTO;
using Contracts.DTO.HistoriesDto;
using Contracts.Exceptions;
using Contracts.Interfaces.Business.HistoryBusinessLogicContracts;
using Contracts.Interfaces.Storages.HistoryStorageContracts;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.HistoryBusinessImp;

public class OrganisationHistoryBusinessLogic(
    IOrganisationHistoryStorageContract storage,
    ILogger<OrganisationHistoryBusinessLogic> logger)
    : IOrganisationHistoryBusinessLogic
{
    public List<OrganisationHistoryDto> GetHistory(string organisationId)
    {
        if (string.IsNullOrWhiteSpace(organisationId))
            throw new ValidationException("OrganisationId is empty");

        logger.LogInformation("Getting organisation history. OrganisationId={OrganisationId}", organisationId);

        return storage.GetByOrganisationId(organisationId) ?? [];
    }

    public OrganisationDto GetAsOf(string organisationId, DateTime atUtc)
    {
        if (string.IsNullOrWhiteSpace(organisationId))
            throw new ValidationException("OrganisationId is empty");

        if (atUtc.Kind != DateTimeKind.Utc)
            atUtc = atUtc.ToUniversalTime();

        logger.LogInformation("Getting organisation as-of. OrganisationId={OrganisationId}, AtUtc={AtUtc:o}", organisationId, atUtc);

        return storage.GetAsOf(organisationId, atUtc);
    }

    public void RestoreFromHistory(string historyId)
    {
        if (string.IsNullOrWhiteSpace(historyId))
            throw new ValidationException("HistoryId is empty");

        logger.LogInformation("Restoring organisation from history. HistoryId={HistoryId}", historyId);

        storage.RestoreFromHistory(historyId);
    }
}
