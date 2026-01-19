namespace Contracts.DTO;

public class ChartOfAccountDto
{
    public string? Id { get; set; }

    public required string NumChart { get; set; } 

    public required string Name { get; set; } 

    public string? Subconto1 {  get; set; } 
    public string? Subconto2 { get; set; } 

    public bool IsDeleted { get; set; } 

}
