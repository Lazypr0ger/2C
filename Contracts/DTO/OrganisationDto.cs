using System.Globalization;

namespace Contracts.DTO;

public class OrganisationDto(string id, string name, bool isDeleted)
{
    public string Id { get; set; } = id;

    public required string Name { get; set; } = name;

    public bool IsDeleted { get; set; } = isDeleted;


}
