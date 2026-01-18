using Contracts.Enums;
using System.Xml.Linq;

namespace Contracts.ViewModels;

public class ProductionVM
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Code { get; set; }
    public TypeProduct Type { get; set; }
    public required string Name { get; set; } 
    public decimal PlannedCost { get; set; } 
    public required string DepartamentId { get; set; } 
    public bool IsDeleted { get; set; } 
}
