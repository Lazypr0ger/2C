using Contracts.Enums;

namespace DataBase.Entities;

public class Operation
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string NameDocument { get; set; }

    public TypeDocument Type { get; set; }

    public DateTime DateOperation { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? OrganisationId { get; set; }
    public Organisation? Organisation { get; set; }

    public string? DepartamentId { get; set; }
    public Departament? Departament { get; set;}

    public decimal TotalAmountDocument { get; set; }

    public required string Agent { get; set; }

    public required List<TransactionLog> TransactionLog { get; set; }

    public List<Element>? Element { get; set; }

    public bool IsDeleted { get; set; }
}
