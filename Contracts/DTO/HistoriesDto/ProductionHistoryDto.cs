using Contracts.Enums;

namespace Contracts.DTO.HistoriesDto;

public class ProductionHistoryDto(string productionId, string oldCode, TypeProduct oldType,string oldName, decimal oldPlannedCost)
{
    public string ProductionId { get; set; } = productionId;
    public string? OldCode { get; set; } = oldCode;

    public TypeProduct? OldType { get; set; } = oldType;
    public string? OldName { get; set; } = oldName;

    public decimal? OldPlannedCost { get; set; } = oldPlannedCost;

    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}
