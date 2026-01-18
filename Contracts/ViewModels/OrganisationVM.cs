namespace Contracts.ViewModels;

public class OrganisationVM
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }

    public required string AccountNumOrg { get; set; }
    public bool IsDeleted { get; set; }
}
