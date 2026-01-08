using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Entities;

public class TransactionLog
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public DateTime DateOperation { get; set; }

    public decimal Subconto1Deb { get; set; }

    public decimal Subconto2Deb { get; set; }
    public decimal Subconto1Cred { get; set; }

    public decimal Subconto2Cred { get; set; }

    public decimal Amount { get; set; }

    public int Count { get; set; }

    public string? Comment { get; set; }

    [ForeignKey("OperationId")]
    public Operation? Operation { get; set; }
    
    public required string ChartOfAccountId { get; set; }
    public ChartOfAccount? ChartOfAccount { get; set; }

    public required string ChartOfAccount2Id { get; set; }
    public ChartOfAccount? ChartOfAccount2 { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted { get; set; }
}
