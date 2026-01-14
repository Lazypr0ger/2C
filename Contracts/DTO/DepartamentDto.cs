namespace Contracts.DTO;

public class DepartamentDto(string id, string name, List<ProductionDto> production, bool isDeleted)
{
    public string Id { get; set; } = id;

    public string Name { get; set; } = name;

    public List<ProductionDto>? Production { get; set; } = production;

    public bool IsDeleted { get; set; } = isDeleted;
}
