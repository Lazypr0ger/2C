using Contracts.Enums;

namespace Contracts.DTO;

public class OperationDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string NameDocument { get; set; }

    public TypeDocument Type {  get; set; }

    public DateTime DateOperation { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? OrganisationId { get; set; }
    public string? DepartamentId { get; set; }
    public double TotalAmountDocument { get; set; }

    public required string Agent {  get; set; }

}
