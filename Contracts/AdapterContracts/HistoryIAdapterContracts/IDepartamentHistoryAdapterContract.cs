using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.AdapterContracts.OperationResponses.HistoryOperationResponcse;

namespace Contracts.AdapterContracts.HistoryIAdapterContracts
{
    public interface IDepartamentHistoryAdapterContract
    {
        DepartamentHistoryOperationResponse GetHistory(string departamentId);

        DepartamentHistoryOperationResponse GetAsOf(string departamentId, DateTime atUtc);
    }
}
