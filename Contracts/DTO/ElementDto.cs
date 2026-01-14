namespace Contracts.DTO;

public class ElementDto(string id, ProductionDto productionElement, int countProduct, decimal costRealisation, decimal TotalCostElement, bool isDeleted)
{
    public string Id { get; set; } = id;

    public ProductionDto ProductionElement = productionElement;

    public int CountProduct { get; set; } = countProduct;

    public decimal CostRealisation { get; set; } = costRealisation;

    public decimal TotalCostElement { get; set; } = TotalCostElement;

    public bool IsDeleted { get; set; } = isDeleted;
}
