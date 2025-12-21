namespace Contracts.DTO;

public class TransactionLogDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public DateTime DateOperation { get; set; }

    public double Subconto1Deb { get; set; }

    public double Subconto2Deb { get; set; }
    public double Subconto1Cred { get; set; }

    public double Subconto2Cred { get; set; }

    public double Amount { get; set; }

    public int Count { get; set; }

    public string? Comment { get; set; }

}
