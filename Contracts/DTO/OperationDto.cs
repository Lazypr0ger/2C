using Contracts.Enums;

namespace Contracts.DTO;

public class OperationDto(string id, string nameDocument,
    TypeDocument typeDocument, DateTime dateOperation,
    DateTime startDate, DateTime endDate,decimal totalAmountDocument, string agent, bool isDeleted)
{

    public string Id { get; set; } = id;

    public required string NameDocument { get; set; } = nameDocument;

    public TypeDocument Type {  get; set; } = typeDocument;

    public DateTime DateOperation { get; set; } = dateOperation;

    public decimal TotalAmountDocument { get; set; } = totalAmountDocument;

    public required string Agent { get; set; } = agent;
    public bool IsDeleted { get; set; } = isDeleted;

    public DateTime? StartDate { get; set; } = startDate;
    public DateTime? EndDate { get; set; } = endDate;

    public string? OrganisationId { get; set; }
    public string? DepartamentId { get; set; }

}
