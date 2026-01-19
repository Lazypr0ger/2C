using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Entities;

public class Element
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();

    public required string OperationId { get; set; }
    [ForeignKey(nameof(OperationId))]
    public Operation? Operation { get; set; }

    public required string ProductionId { get; set; }
    [ForeignKey(nameof(ProductionId))]
    public Production? Production { get; set; }

    public int CountElement { get; set; }

    // цена продажи (для реализации)
    public decimal? Price { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted { get; set; } = false;
}
