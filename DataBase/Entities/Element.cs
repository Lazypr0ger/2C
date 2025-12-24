namespace DataBase.Entities;

public class Element
{
    public required string Id { get; set; } 

    public int CountProduct { get; set; }

    public double CostRealisation { get; set; }

    public double TotalCostElement { get; set; }

    public required Production Production { get; set; }

    public required Operation Operation { get; set; }
}
