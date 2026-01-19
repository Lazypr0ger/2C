using Contracts.AdapterContracts.OperationResponses;
using Contracts.AdapterContracts.OperationResponses.HistoryOperationResponcse;

namespace Contracts.AdapterContracts.HistoryIAdapterContracts;

public interface IOrganisationHistoryAdapterContract
{
    OrganisationHistoryOperationResponse GetHistory(string organisationId);
    OrganisationHistoryOperationResponse GetAsOf(string organisationId, DateTime atUtc);
    OrganisationHistoryOperationResponse RestoreFromHistory(string historyId);
}
