namespace Contracts.DTO;

public class ChartOfAccountDto(string id, string numChart, string name, string subconto1, string subconto2, bool isDeleted)
{
    private readonly DepartamentDto? _departament;
    public string Id { get; set; } = id;

    public required string NumChart { get; set; } = numChart;

    public required string Name { get; set; } = name;

    public string? Subconto1 {  get; set; } = subconto1;
    public string? Subconto2 { get; set; } = subconto2;

    public bool IsDeleted { get; set; } = isDeleted;

    public List<DepartamentDto>? DepartamentDtos { get; private set; }
    public List<ProductionDto>? ProductionDtos { get; private set; }

}
