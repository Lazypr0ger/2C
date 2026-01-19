
using System.Xml.Linq;

namespace Contracts.BindingModels;

public class OrganisationBM
{
    public string? Id { get; set; }

    public string? Name { get; set; }
    public string? AccountNumOrg { get; set; } 
    public bool IsDeleted { get; set; }

}
