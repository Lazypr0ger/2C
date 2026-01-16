namespace Contracts.ViewModels;

public class ElementVM
{
    public required string Id { get; set; }

    public int CountProduct { get; set; } 

    public decimal CostRealisation { get; set; } 

    public string? ProductionId { get; set; }
    public string? OperationId { get; set; }

    public bool IsDeleted { get; set; }
}
