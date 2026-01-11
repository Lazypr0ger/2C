using System.Xml.Linq;

namespace _2Cclient.ViewModels;

public class ChartOfAccountVM
{
    public required string Id { get; set; }

    public required string NumChart { get; set; }

    public required string Name { get; set; }

    public string? Subconto1 { get; set; } 
    public string? Subconto2 { get; set; } 

    public bool IsDeleted { get; set; } 
}
