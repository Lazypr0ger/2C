namespace Contracts.DTO;

public class OrganisationDto(string id, string name, string chartOfAccountId, bool isDeleted)
{

    public string Id { get; set; } = id;

    public required string Name { get; set; } = name;
    public string ChartOfAccountId { get; set; } = chartOfAccountId;
    public required string AccountNumOrg { get; set; }
    public bool IsDeleted { get; set; } = isDeleted;


}
