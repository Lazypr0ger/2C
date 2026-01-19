using Contracts.DTO;
using Contracts.DTO.HistoriesDto;

namespace Contracts.Interfaces.Business.HistoryBusinessLogicContracts;

public interface IProductionHistoryBusinessLogic
{
    List<ProductionHistoryDto> GetHistory(string productionId);
    ProductionDto GetAsOf(string productionId, DateTime atUtc);
    void RestoreFromHistory(string historyId);
}
