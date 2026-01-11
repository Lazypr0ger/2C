namespace Contracts.DTO;

public class ElementDto(string id, int countProduct, decimal costRealisation, decimal TotalCostElement, bool isDeleted)
{
    private readonly ProductionDto? _production;
    public string Id { get; set; } = id;

    public string ProductionElement => _production?.Name ?? string.Empty;

    public int CountProduct { get; set; } = countProduct;

    public decimal CostRealisation { get; set; } = costRealisation;

    public decimal TotalCostElement { get; set; } = TotalCostElement;

    public bool IsDeleted { get; set; } = isDeleted;
}
