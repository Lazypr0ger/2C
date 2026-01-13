using System.Globalization;
using Contracts.Enums;

namespace Contracts.DTO;

public class ProductionDto(string id, string code, TypeProduct typeProduct, string name, decimal plannedCost, bool isDeleted)
{
    public string Id { get; set; } = id;
    public string Code { get; set; } = code;

    public TypeProduct TypeProduct { get; set; } = typeProduct;
    public string Name { get; set; } = name;

    public decimal PlannedCost { get; set; } = plannedCost;
    public bool IsDeleted { get; set; } = isDeleted;
}
