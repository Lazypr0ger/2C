using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.DTO.HistoriesDto;
using Contracts.DTO;

namespace Contracts.Interfaces.Storages.HistoryStorageContracts
{
    public interface IDepartamentHistoryStorageContract
    {
        List<DepartamentHistoryDto> GetByDepartamentId(string departamentId);

        DepartamentDto GetAsOf(string departamentId, DateTime atUtc);

        void RestoreFromHistory(string historyId);

    }
}
