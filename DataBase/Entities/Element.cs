using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Entities;

public class Element
{
    public required string Id { get; set; }  = Guid.NewGuid().ToString();

    public int CountProduct { get; set; }

    public decimal CostRealisation { get; set; }

    public decimal TotalCostElement { get; set; }

    [ForeignKey("ProductionId")]
    public Production? Production { get; set; }

    [ForeignKey("OperationId")]
    public Operation? Operation { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted { get; set; } 
}
