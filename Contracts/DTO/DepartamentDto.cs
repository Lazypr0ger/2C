namespace Contracts.DTO;

public class DepartamentDto(string id, string name, bool isDeleted)
{
    private readonly ChartOfAccountDto? _chartOfAccountDto;
    public string Id { get; set; } = id;

    public required string Name { get; set; } = name;

    public string DepChartNum => _chartOfAccountDto?.NumChart ?? string.Empty;

    public List<ProductionDto>? ProductionDtos { get; set; }

    public bool IsDeleted { get; set; } = isDeleted;
}
