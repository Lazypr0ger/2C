namespace DataBase.Entities;

public class Departament
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string ChartChartOfAccountId { get; set; }
    public required ChartOfAccount ChartOfAccount { get; set; }

    public List<Production>? Productions { get; set; }
}
