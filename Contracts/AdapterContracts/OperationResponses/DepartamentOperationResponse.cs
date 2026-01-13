using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Infrastructure;
using Contracts.ViewModels;

namespace Contracts.AdapterContracts.OperationResponses
{
    public class DepartamentOperationResponse : OperationResponse
    {
        public static DepartamentOperationResponse OK(List<DepartamentVM> data) => OK<DepartamentOperationResponse, List<DepartamentVM>>(data);

        public static DepartamentOperationResponse OK(DepartamentVM data) => OK<DepartamentOperationResponse, DepartamentVM>(data);

        public static DepartamentOperationResponse NoContent() => NoContent<DepartamentOperationResponse>();

        public static DepartamentOperationResponse BadRequest(string message) => BadRequest<DepartamentOperationResponse>(message);

        public static DepartamentOperationResponse NotFound(string message) => NotFound<DepartamentOperationResponse>(message);

        public static DepartamentOperationResponse InternalServerError(string message) => InternalServerError<DepartamentOperationResponse>(message);
    }
}
