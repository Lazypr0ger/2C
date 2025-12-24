using System.ComponentModel.DataAnnotations;

namespace DataBase.Entities;

public class ChartOfAccount
{
    public required string Id { get; set; }

    public required string NumChart {  get; set; }

    public required string Name { get; set; }

    public string? Subconto1 { get; set; } 
    public string? Subconto2 { get; set; } 

    public List<Departament>? Departaments { get; set; }

    public List<Production>? Productions { get; set; }

    public List<Organisation>? Organisations { get; set; } 

    public List<TransactionLog>? TransactionLogs1 { get; set; }

    public List<TransactionLog>? TransactionLogs2 { get; set; }
}
