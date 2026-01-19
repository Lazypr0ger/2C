using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
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

    [ForeignKey("OrganisationId")]
    public Organisation? Organisation { get; set; }

    
    public string? DepartamentId { get; set; }

    [ForeignKey("DepartamentId")]
    public Departament? Departament { get; set;}

    
    public decimal? TotalAmountDocument { get; set; }

    public List<TransactionLog>? TransactionLog { get; set; }

    public List<Element>? Element { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted { get; set; }
}
