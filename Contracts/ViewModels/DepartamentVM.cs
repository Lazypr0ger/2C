namespace Contracts.ViewModels;

public class DepartamentVM
{
    public string? Id { get; set; } = Guid.NewGuid().ToString();

    public required string Name { get; set; } 
    public bool IsDeleted { get; set; }
}
