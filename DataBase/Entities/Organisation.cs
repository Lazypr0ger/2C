using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DataBase.Entities.HistoriesModel;

namespace DataBase.Entities;

public class Organisation
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string Name { get; set; }

    public required string AccountNumOrg { get; set; }
    public List<Operation>? Operation { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted { get; set; } = false;

    public List<OrganisationHistory>? OrganisationHistories { get; set; }


}
