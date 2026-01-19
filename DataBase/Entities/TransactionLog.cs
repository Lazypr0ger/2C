using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Entities;

public class TransactionLog
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public DateTime DateOperation { get; set; }

    //subconto
    public string? Subconto1Deb { get; set; }

    public string? Subconto2Deb { get; set; }
    public string? Subconto1Cred { get; set; }

    public string? Subconto2Cred { get; set; }

    //amount
    public decimal Amount { get; set; }

    public int Count { get; set; }

    public string? Comment { get; set; }


    //operation link
    public string? OperationId { get; set; }
    
    [ForeignKey("OperationId")]
    public Operation? Operation { get; set; }
    

    //debit
    public required string ChartOfAccountDebId { get; set; }
    [ForeignKey("ChartOfAccountDebId")]
    public ChartOfAccount? ChartOfAccountDeb { get; set; }


    //credit
    public required string ChartOfAccountCredId { get; set; }
    [ForeignKey("ChartOfAccountCredId")]
    public ChartOfAccount? ChartOfAccountCred { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted { get; set; } = false;    
}
