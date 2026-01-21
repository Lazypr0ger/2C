namespace Contracts.BindingModels;

public class TransactionLogBM
{
    public string? Id { get; set; }

    public DateTime DateOperation { get; set; }

    public string? Subconto1Deb { get; set; }
    public string? Subconto2Deb { get; set; }
    public string? Subconto1Cred { get; set; }
    public string? Subconto2Cred { get; set; }

    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }

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

    public string? ChartOfAccountDebId { get; set; }
    public string? ChartOfAccountCredId { get; set; }

    public bool IsDeleted { get; set; }
}
