using System.Xml.Linq;

namespace Contracts.ViewModels;

public class TransactionLogVM
{
    public required string Id { get; set; }

    public DateTime DateOperation { get; set; } 
    public decimal Amount { get; set; } 

    public int Count { get; set; } 
    public required string ChartOfAccountDebId { get; set; } 
    public required string ChartOfAccountCredId { get; set; } 

    public bool IsDeleted { get; set; } 

    public decimal Subconto1Deb { get; set; } 

    public decimal Subconto2Deb { get; set; } 
    public decimal Subconto1Cred { get; set; }

    public decimal Subconto2Cred { get; set; } 
    public string? Comment { get; set; } 
}
