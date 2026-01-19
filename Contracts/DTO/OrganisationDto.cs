namespace Contracts.DTO;

public class OrganisationDto
{

    public string? Id { get; set; }

    public required string Name { get; set; }
    public required string AccountNumOrg { get; set; }
    public bool IsDeleted { get; set; }


}
