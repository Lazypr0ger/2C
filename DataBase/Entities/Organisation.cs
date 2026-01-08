using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Entities;

public class Organisation
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string Name { get; set; }

    public required string AccountNumOrg { get; set; }

    public List<Operation>? Operation { get; set; }

    [ForeignKey("ChartOfAccountId")]
    public required ChartOfAccount ChartOfAccount { get; set; }

    public bool IsDeleted { get; set; }
}
