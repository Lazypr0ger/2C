namespace Contracts.DTO;

public class DepartamentDto(string id, string name)
{
    public string Id { get; set; } = id;

    public required string Name { get; set; } = name;
}
