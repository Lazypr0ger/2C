namespace Contracts.ViewModels;

public class ElementVM
{
    public string? Id { get; set; }

    public required string OperationId { get; set; }

    public required string ProductionId { get; set; }

    public int CountElement { get; set; }

    public decimal? Price { get; set; }

    public bool IsDeleted { get; set; }
}
