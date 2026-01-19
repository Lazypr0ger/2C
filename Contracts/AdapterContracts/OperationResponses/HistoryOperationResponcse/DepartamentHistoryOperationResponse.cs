using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.ViewModels;
using Contracts.ViewModels.HistoryModels;

namespace Contracts.AdapterContracts.OperationResponses.HistoryOperationResponcse
{
    public class DepartamentHistoryOperationResponse : OperationResponse
    {
        public static DepartamentHistoryOperationResponse OK(List<DepartamentHistoryVM> data)
         => OK<DepartamentHistoryOperationResponse, List<DepartamentHistoryVM>>(data);

        public static DepartamentHistoryOperationResponse OK(DepartamentVM data)
            => OK<DepartamentHistoryOperationResponse, DepartamentVM>(data);

        public static DepartamentHistoryOperationResponse NoContent()
            => NoContent<DepartamentHistoryOperationResponse>();

        public static DepartamentHistoryOperationResponse BadRequest(string message)
            => BadRequest<DepartamentHistoryOperationResponse>(message);

        public static DepartamentHistoryOperationResponse NotFound(string message)
            => NotFound<DepartamentHistoryOperationResponse>(message);

        public static DepartamentHistoryOperationResponse InternalServerError(string message)
            => InternalServerError<DepartamentHistoryOperationResponse>(message);
    }
}
