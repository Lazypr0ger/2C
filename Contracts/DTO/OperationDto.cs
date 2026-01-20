using Contracts.Enums;

namespace Contracts.DTO;

public class OperationDto
{
    public string? Id { get; set; }
    public string? NameDocument { get; set; }
    public DateTime DateOperation { get; set; }
    public OperationType Type { get; set; }

    public decimal TotalAmountDocument { get; set; }

    public string? Comment { get; set; }

    public string? OrganisationId { get; set; }
    public string? DepartamentId { get; set; }

    public bool IsDeleted { get; set; }

    public List<ElementDto> Elements { get; set; } = new();
}
