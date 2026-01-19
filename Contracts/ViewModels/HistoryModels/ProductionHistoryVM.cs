using Contracts.Enums;

namespace Contracts.ViewModels.HistoryModels;

public class ProductionHistoryVM
{
    public string? Id { get; set; } 
    public string? ProductionId { get; set; } 

    public string? Code { get; set; } 
    public string? Name { get; set; } 
    public decimal PlannedCost { get; set; }
    public TypeProduct Type { get; set; }
    public string? DepartamentId { get; set; } 
    public bool IsDeleted { get; set; }

    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}
