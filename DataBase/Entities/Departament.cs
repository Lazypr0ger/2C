namespace DataBase.Entities;

public class Departament
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string Name { get; set; }

    public required ChartOfAccount ChartOfAccount { get; set; }

    public List<Production>? Production { get; set; }
}
