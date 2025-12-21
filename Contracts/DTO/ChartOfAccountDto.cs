namespace Contracts.DTO;

public class ChartOfAccountDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string NumChart { get; set; }

    public required string Name { get; set; } 

    public string? Subconto1 {  get; set; } = string.Empty;
    public string? Subconto2 { get; set; } = string.Empty;
}
