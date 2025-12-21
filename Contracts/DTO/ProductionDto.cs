using Contracts.Enums;

namespace Contracts.DTO;

public class ProductionDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Code { get; set; }

    public TypeProduct Type { get; set; }
    public required string Name { get; set; }

    public double PlannedCost { get; set; }

}
