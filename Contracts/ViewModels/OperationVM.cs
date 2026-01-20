using Contracts.Enums;

namespace Contracts.ViewModels;

public class OperationVM
{
    public required string Id { get; set; }
    public required string NameDocument { get; set; }
    public DateTime DateOperation { get; set; }
    public OperationType Type { get; set; }

    public string? Comment { get; set; } 

    public decimal TotalAmountDocument { get; set; }

    public string? OrganisationId { get; set; }
    public string? OrganisationName { get; set; }
    public string? DepartamentId { get; set; }
    public string? DepartamentName { get; set; }  

    public bool IsDeleted { get; set; }

    public List<ElementVM> Elements { get; set; } = new();
}
