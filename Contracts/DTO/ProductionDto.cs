using System.Globalization;
using Contracts.Enums;

namespace Contracts.DTO;

public class ProductionDto(string id, string code, TypeProduct typeProduct, string name, decimal plannedCost, bool isDeleted)
{
    private readonly DepartamentDto? _departament;
    private readonly ChartOfAccountDto? _chartOfAccount;
    public string Id { get; set; } = id;
    public required string Code { get; set; } = code;

    public TypeProduct TypeProduct { get; set; } = typeProduct;
    public required string Name { get; set; } = name;

    public decimal PlannedCost { get; set; } = plannedCost;
    public bool IsDeleted { get; set; } = isDeleted;

    public string ProductChart => _chartOfAccount?.NumChart ?? string .Empty;
    public string DepartamentName => _departament?.Name ?? string.Empty;

}
