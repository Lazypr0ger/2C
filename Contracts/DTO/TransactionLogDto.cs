namespace Contracts.DTO;

public class TransactionLogDto(string id, DateTime dateOperation, decimal subconto1Deb,
    decimal subconto2Deb, decimal subconto1Cred, decimal subconto2Cred,decimal amount, int count,
    string chartOfAccountDebId, string chartOfAccountCredId, string comment, bool isDeleted)
{
    public string Id { get; set; } = id;

    public DateTime DateOperation { get; set; } = dateOperation;
    public decimal Amount { get; set; } = amount;

    public int Count { get; set; } = count;
    public string ChartOfAccountDebId { get; set; } = chartOfAccountDebId;
    public string ChartOfAccountCredId { get; set; } = chartOfAccountCredId;

    public bool IsDeleted { get; set; } = isDeleted;

    public decimal Subconto1Deb { get; set; } = subconto1Deb;

    public decimal Subconto2Deb { get; set; } = subconto2Deb;
    public decimal Subconto1Cred { get; set; } = subconto1Cred;

    public decimal Subconto2Cred { get; set; } = subconto2Cred;
    public string? Comment { get; set; } = comment;
}
