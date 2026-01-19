using Contracts.DTO;
using Contracts.DTO.HistoriesDto;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Business.HistoryBusinessLogicContracts;
using Contracts.Interfaces.Storages;
using Contracts.Interfaces.Storages.HistoryStorageContracts;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.HistoryBusinessImp;

public class DepartamentHistoryBusinessLogic(
    IDepartamentHistoryStorageContract departamentHistoryStorage,
    ILogger<DepartamentHistoryBusinessLogic> logger)
    : IDepartamentHistoryBusinessLogic
{
    public DepartamentDto GetAsOf(string departamentId, DateTime atUtc)
    {
        if (string.IsNullOrWhiteSpace(departamentId))
            throw new ValidationException("DepartamentId is empty");

        if (atUtc.Kind != DateTimeKind.Utc)
            atUtc = atUtc.ToUniversalTime();

        logger.LogInformation("Getting departament as-of. DepartamentId={DepartamentId}, AtUtc={AtUtc:o}", departamentId, atUtc);

        return departamentHistoryStorage.GetAsOf(departamentId, atUtc);
    }

    public List<DepartamentHistoryDto> GetHistory(string departamentId)
    {
        if (string.IsNullOrWhiteSpace(departamentId))
            throw new ValidationException("DepartamentId is empty");

        logger.LogInformation("Getting departament history. DepartamentId={DepartamentId}", departamentId);

        var list = departamentHistoryStorage.GetByDepartamentId(departamentId);

        return list ?? [];
    }

    public void RestoreFromHistory(string historyId)
    {
        if (string.IsNullOrWhiteSpace(historyId))
            throw new ValidationException("HistoryId is empty");

        logger.LogInformation("Restoring departament from history. HistoryId={HistoryId}", historyId);

        departamentHistoryStorage.RestoreFromHistory(historyId);
    }

}
