using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.ViewModels.HistoryModels
{
    public class DepartamentHistoryVM
    {
        public string? Id { get; set; } 
        public string? DepartamentId { get; set; }
        public string? Name { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
