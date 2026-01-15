using Contracts.Enums;

namespace Contracts.ViewModels;

public class OperationVM
{
    public required string Id { get; set; } 

    public required string NameDocument { get; set; }

    public TypeDocument Type { get; set; } 

    public DateTime DateOperation { get; set; } 

    public decimal TotalAmountDocument { get; set; } 

    public required string Agent { get; set; } 
    public bool IsDeleted { get; set; } 

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; } 

    public string? OrganisationId { get; set; }
    public string? DepartamentId { get; set; }
}
