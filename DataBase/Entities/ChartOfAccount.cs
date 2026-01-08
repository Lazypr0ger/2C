using System.ComponentModel.DataAnnotations;

namespace DataBase.Entities;

public class ChartOfAccount
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string NumChart {  get; set; }

    public required string Name { get; set; }

    public string? Subconto1 { get; set; } 
    public string? Subconto2 { get; set; } 

    public List<Departament>? Departament { get; set; }

    public List<Production>? Production { get; set; }

    public List<Organisation>? Organisation { get; set; } 

    public List<TransactionLog>? TransactionLog1 { get; set; }

    public List<TransactionLog>? TransactionLog2 { get; set; }
}
