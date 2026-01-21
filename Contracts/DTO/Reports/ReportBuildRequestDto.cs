using Contracts.Enums;

namespace Contracts.DTO.Reports;

public class ReportBuildRequestDto
{
    public ReportTypeCodes TypeCode { get; set; }

    public DateTime From { get; set; }
    public DateTime To { get; set; }

    public string? Name { get; set; }
    public string? Comment { get; set; }
}
