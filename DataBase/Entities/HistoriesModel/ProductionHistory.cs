using Contracts.Enums;
using System.Xml.Linq;

namespace DataBase.Entities.HistoriesModel;

public class ProductionHistory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string ProductionId { get; set; } 
    public string? OldCode { get; set; } 
    public TypeProduct? OldType { get; set; }
    public string? OldName { get; set; }

    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }

    public Production? Production { get; set; }
}
