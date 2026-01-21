using Contracts.Enums;

namespace Contracts.DTO.Reports;

public class ReportListItemDto
{
    public string Id { get; set; } = string.Empty;
    public ReportTypeCodes TypeCode { get; set; }
    public string Name { get; set; } = string.Empty;

    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public DateTime BuildDate { get; set; }

    public decimal? TotalActualCosts { get; set; }
    public decimal Total1 { get; set; }
    public decimal Total2 { get; set; }
    public decimal Total3 { get; set; }
}
