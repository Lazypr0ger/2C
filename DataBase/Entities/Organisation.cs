using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Entities;

public class Organisation
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string Name { get; set; }

    public required string AccountNumOrg { get; set; }

    [ForeignKey("OrganisationId")]
    public List<Operation>? Operation { get; set; }
    public bool IsDeleted { get; set; }
}
