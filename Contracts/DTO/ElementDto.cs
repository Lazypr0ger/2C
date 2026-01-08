namespace Contracts.DTO;

public class ElementDto(string id, int countProduct, decimal costRealisation, decimal TotalCostElement)
{
    public string Id { get; set; } = id;

    public int CountProduct { get; set; } = countProduct;

    public decimal CostRealisation { get; set; } = costRealisation;

    public decimal TotalCostElement { get; set; } = TotalCostElement;


}
