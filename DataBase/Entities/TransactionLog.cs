using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Entities;

public class TransactionLog
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public DateTime DateOperation { get; set; }

    public string? Subconto1Deb { get; set; }
    public string? Subconto2Deb { get; set; }
    public string? Subconto1Cred { get; set; }
    public string? Subconto2Cred { get; set; }

    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }

    [NotMapped]
    public decimal Amount
    {
        get => DebitAmount;
        set
        {
            DebitAmount = value;
            CreditAmount = value;
        }
    }
    public int Count { get; set; }
    public string? Comment { get; set; }

    public string? OperationId { get; set; }
    [ForeignKey(nameof(OperationId))]
    public Operation? Operation { get; set; }

    public required string ChartOfAccountDebId { get; set; }
    [ForeignKey(nameof(ChartOfAccountDebId))]
    public ChartOfAccount? ChartOfAccountDeb { get; set; }

    public required string ChartOfAccountCredId { get; set; }
    [ForeignKey(nameof(ChartOfAccountCredId))]
    public ChartOfAccount? ChartOfAccountCred { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted { get; set; } = false;
}
