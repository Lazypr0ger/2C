using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace DataBase.Entities;

public class Departament
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string Name { get; set; }


    [ForeignKey("ChartOfAccountId")]
    public required ChartOfAccount ChartOfAccount { get; set; }

    public List<Production>? Production { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted { get; set; }
}
