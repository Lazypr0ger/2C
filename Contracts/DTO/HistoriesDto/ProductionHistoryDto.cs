using Contracts.Enums;

namespace Contracts.DTO.HistoriesDto;

public class ProductionHistoryDto
{
    public string? Id {get; set;}
    public string? ProductionId { get; set; }
    public string? Code { get; set; }

    public TypeProduct? Type { get; set; } 
    public string? Name { get; set; } 

    public decimal? PlannedCost { get; set; }

    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}
