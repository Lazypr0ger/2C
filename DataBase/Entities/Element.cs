namespace DataBase.Entities;

public class Element
{
    public required string Id { get; set; }  = Guid.NewGuid().ToString();

    public int CountProduct { get; set; }

    public decimal CostRealisation { get; set; }

    public decimal TotalCostElement { get; set; }

    public required Production Production { get; set; }

    public required Operation Operation { get; set; }
}
