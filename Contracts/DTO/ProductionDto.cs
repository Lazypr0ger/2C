using System.Globalization;
using Contracts.Enums;

namespace Contracts.DTO;

public class ProductionDto(string id, string code,
    TypeProduct type, string name, decimal plannedCost,
    string departamentId, bool isDeleted)
{
    public string Id { get; set; } = id;
    public string Code { get; set; } = code;
    public TypeProduct Type { get; set; } = type;
    public string Name { get; set; } = name;
    public decimal PlannedCost { get; set; } = plannedCost;
    public string DepartamentId { get; set; } = departamentId;
    public bool IsDeleted { get; set; } = isDeleted;
}
