using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.ViewModels.HistoryModels
{
    public class OrganisationHistoryVM
    {
        public string? Id { get; set; }
        public string? OrganisationId { get; set; }
        public string? Name { get; set; }

        public string? AccountNumOrg { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
