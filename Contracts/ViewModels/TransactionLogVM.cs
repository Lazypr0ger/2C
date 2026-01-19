namespace Contracts.ViewModels;

public class TransactionLogVM
{
    public required string Id { get; set; }

    public DateTime DateOperation { get; set; }

    public string? Subconto1Deb { get; set; }
    public string? Subconto2Deb { get; set; }
    public string? Subconto1Cred { get; set; }
    public string? Subconto2Cred { get; set; }

    public decimal Amount { get; set; }
    public int Count { get; set; }

    public string? Comment { get; set; }

    public string? OperationId { get; set; }

    public required string ChartOfAccountDebId { get; set; }
    public required string ChartOfAccountCredId { get; set; }

    public bool IsDeleted { get; set; }
}
