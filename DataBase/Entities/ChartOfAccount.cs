using System.ComponentModel.DataAnnotations;

namespace DataBase.Entities;

public class ChartOfAccount
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string NumChart {  get; set; }

    public required string Name { get; set; }

    public string? Subconto1 { get; set; } = string.Empty;
    public string? Subconto2 { get; set; } = string.Empty;

    public List<TransactionLog> TransactionLog1 { get; set; } = new();

    public List<TransactionLog> TransactionLog2 { get; set; } = new();
    public bool IsDeleted { get; set; }
}
