using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.DTO.HistoriesDto;
using Contracts.DTO;

namespace Contracts.Interfaces.Business.HistoryBusinessLogicContracts
{
    public interface IDepartamentHistoryBusinessLogic
    {
        List<DepartamentHistoryDto> GetHistory(string departamentId);
        DepartamentDto GetAsOf(string departamentId, DateTime atUtc);
    }
}
