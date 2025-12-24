namespace DataBase.Entities;

public class TransactionLog
{
    public required string Id { get; set; }

    public DateTime DateOperation { get; set; }

    public double Subconto1Deb { get; set; }

    public double Subconto2Deb { get; set; }
    public double Subconto1Cred { get; set; }

    public double Subconto2Cred { get; set; }

    public double Amount { get; set; }

    public int Count { get; set; }

    public string? Comment { get; set; }

    public required Operation Operation { get; set; }

    public required ChartOfAccount ChartOfAccount { get; set; }
}
