namespace Contracts.DTO;

public class OrganisationDto(string id, string name, string AccountNumOrg, bool isDeleted)
{

    public string Id { get; set; } = id;

    public required string Name { get; set; } = name;
    public required string AccountNumOrg { get; set; } = AccountNumOrg;
    public bool IsDeleted { get; set; } = isDeleted;


}
