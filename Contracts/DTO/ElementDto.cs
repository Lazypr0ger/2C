namespace Contracts.DTO;

public class ElementDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public int CountProduct { get; set; }

    public double CostRealisation { get; set; }

    public double TotalCostElement { get; set; }


}
