namespace Contracts.ViewModels;

public class DepartamentVM
{
    public required string Id { get; set; }
    public required string Name { get; set; } 
    public bool IsDeleted { get; set; }
}
