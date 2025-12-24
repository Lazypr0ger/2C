namespace Contracts.DTO;

public class TransactionLogDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public DateTime DateOperation { get; set; }

    public decimal Subconto1Deb { get; set; }

    public decimal Subconto2Deb { get; set; }
    public decimal Subconto1Cred { get; set; }

    public decimal Subconto2Cred { get; set; }

    public decimal Amount { get; set; }

    public int Count { get; set; }

    public string? Comment { get; set; }

}
