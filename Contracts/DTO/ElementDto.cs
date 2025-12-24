namespace Contracts.DTO;

public class ElementDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public int CountProduct { get; set; }

    public decimal CostRealisation { get; set; }

    public decimal TotalCostElement { get; set; }


}
