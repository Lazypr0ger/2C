using System.Globalization;
using Contracts.Enums;

namespace Contracts.DTO;

public class ProductionDto
{
    public string? Id { get; set; }
    public string? Code { get; set; } 
    public TypeProduct? Type { get; set; } 
    public string? Name { get; set; } 
    public decimal PlannedCost { get; set; } 
    public string? DepartamentId { get; set; } 
    public bool? IsDeleted { get; set; }
}
