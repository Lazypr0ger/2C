using Contracts.Enums;

namespace DataBase.Entities;

public class Production
{
    public required string Id { get; set; }
    public required string Code { get; set; }

    public TypeProduct Type { get; set; }
    public required string Name { get; set; }

    public double PlannedCost { get; set; }

    public required string DepartamentId { get; set; }
    public required Departament Departament { get; set; }

    public List<Element>? Elements { get; set; }

    public required string ChartChartOfAccountId { get; set; }
    public required ChartOfAccount ChartOfAccount { get; set; }
}
