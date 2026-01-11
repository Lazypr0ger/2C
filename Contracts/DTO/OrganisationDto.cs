using System.Globalization;

namespace Contracts.DTO;

public class OrganisationDto(string id, string name, bool isDeleted)
{
    private readonly ChartOfAccountDto? _chartOfAccountDto;
    public string Id { get; set; } = id;

    public required string Name { get; set; } = name;

    public string ChartOfAccountDto => _chartOfAccountDto?.NumChart ?? string.Empty;
    public bool IsDeleted { get; set; } = isDeleted;


}
