using System.Globalization;

namespace Contracts.DTO;

public class OrganisationDto(string id, string name)
{
    public string Id { get; set; } = id;

    public required string Name { get; set; } = name;


}
