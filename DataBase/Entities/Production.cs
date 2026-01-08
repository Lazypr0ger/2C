using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Enums;

namespace DataBase.Entities;

public class Production
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Code { get; set; }

    public TypeProduct Type { get; set; }
    public required string Name { get; set; }

    public decimal PlannedCost { get; set; }

    [ForeignKey("DepartamentId")]
    public required Departament Departament { get; set; }

    public List<Element>? Elements { get; set; }

    [ForeignKey("ChartOfAccountId")]
    public required ChartOfAccount ChartOfAccount { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted { get; set; }
}
