namespace Contracts.DTO;

public class DepartamentDto(string id, string name, bool isDeleted)
{
    public string Id { get; set; } = id;

    public string Name { get; set; } = name;

    public bool IsDeleted { get; set; } = isDeleted;

}
