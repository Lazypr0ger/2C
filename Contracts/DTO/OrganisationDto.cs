namespace Contracts.DTO;

public class OrganisationDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string Name { get; set; }


}
