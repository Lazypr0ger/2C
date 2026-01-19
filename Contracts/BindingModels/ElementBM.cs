namespace Contracts.BindingModels;

public class ElementBM
{
    public string? Id { get; set; }

    public string? ProductionId { get; set; }

    public int CountElement { get; set; }

    public decimal? Price { get; set; }

    public bool IsDeleted { get; set; }
}
