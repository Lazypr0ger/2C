using Contracts.DTO;
using Contracts.DTO.HistoriesDto;

namespace Contracts.Interfaces.Storages.HistoryStorageContracts;

public interface IProductionHistoryStorageContract
{
    List<ProductionHistoryDto> GetByProductionId(string productionId);
    ProductionDto GetAsOf(string productionId, DateTime atUtc);
    void RestoreFromHistory(string historyId);
}
