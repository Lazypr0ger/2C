
using Contracts.DTO;
using System.Xml.Linq;

namespace Contracts.ViewModels;

public class OrganisationVM
{

    public required string Id { get; set;}

    public required string Name { get; set; }

    public required string ChartOfAccountId { get; set; }
    public required string AccountNumOrg { get; set; }
    public bool IsDeleted { get; set; }
}
