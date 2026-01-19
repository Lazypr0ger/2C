using Contracts.DTO;
using Contracts.DTO.HistoriesDto;
using Contracts.Exceptions;
using Contracts.Interfaces.Business.HistoryBusinessLogicContracts;
using Contracts.Interfaces.Storages.HistoryStorageContracts;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.HistoryBusinessImp;

public class ProductionHistoryBusinessLogic(
    IProductionHistoryStorageContract storage,
    ILogger<ProductionHistoryBusinessLogic> logger)
    : IProductionHistoryBusinessLogic
{
    public List<ProductionHistoryDto> GetHistory(string productionId)
    {
        if (string.IsNullOrWhiteSpace(productionId))
            throw new ValidationException("ProductionId is empty");

        logger.LogInformation("Getting production history. ProductionId={ProductionId}", productionId);

        return storage.GetByProductionId(productionId) ?? [];
    }

    public ProductionDto GetAsOf(string productionId, DateTime atUtc)
    {
        if (string.IsNullOrWhiteSpace(productionId))
            throw new ValidationException("ProductionId is empty");

        if (atUtc.Kind != DateTimeKind.Utc)
            atUtc = atUtc.ToUniversalTime();

        logger.LogInformation("Getting production as-of. ProductionId={ProductionId}, AtUtc={AtUtc:o}", productionId, atUtc);

        return storage.GetAsOf(productionId, atUtc);
    }

    public void RestoreFromHistory(string historyId)
    {
        if (string.IsNullOrWhiteSpace(historyId))
            throw new ValidationException("HistoryId is empty");

        logger.LogInformation("Restoring production from history. HistoryId={HistoryId}", historyId);

        storage.RestoreFromHistory(historyId);
    }
}
