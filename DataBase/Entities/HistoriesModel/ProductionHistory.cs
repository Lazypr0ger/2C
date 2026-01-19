using Contracts.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace DataBase.Entities.HistoriesModel;

public class ProductionHistory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string ProductionId { get; set; } 
    public string? Code { get; set; } 
    public TypeProduct Type { get; set; }
    public string? Name { get; set; }
    public decimal PlannedCost { get; set; }
    public string?  DepartamentId { get; set; }

    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    [ForeignKey ("ProductionId")]
    public Production? Production { get; set; }

    public bool IsDeleted { get; set; }
}
