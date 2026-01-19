using Contracts.AdapterContracts.OperationResponses.HistoryOperationResponcse;

namespace Contracts.AdapterContracts.HistoryIAdapterContracts;

public interface IProductionHistoryAdapterContract
{
    ProductionHistoryOperationResponse GetHistory(string productionId);
    ProductionHistoryOperationResponse GetAsOf(string productionId, DateTime atUtc);
    ProductionHistoryOperationResponse RestoreFromHistory(string historyId);
}
