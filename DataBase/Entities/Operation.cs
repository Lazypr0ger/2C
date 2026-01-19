using Contracts.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Entities;

public class Operation
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string NameDocument { get; set; }

    public DateTime DateOperation { get; set; }

    public OperationType Type { get; set; }

    public decimal TotalAmountDocument { get; set; }

    // links
    public string? OrganisationId { get; set; }
    [ForeignKey(nameof(OrganisationId))]
    public Organisation? Organisation { get; set; }

    public string? DepartamentId { get; set; }
    [ForeignKey(nameof(DepartamentId))]
    public Departament? Departament { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted { get; set; } = false;

    // lines + postings
    public List<Element> Element { get; set; } = new();
    public List<TransactionLog> TransactionLog { get; set; } = new();
}
