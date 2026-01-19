namespace Contracts.BindingModels;

public class ChartOfAccountBM
{
    public string? Id { get; set; }
    public string? NumChart { get; set; }
    public string? Name { get; set; }
    public string? Subconto1 { get; set; }
    public string? Subconto2 { get; set; }
    public bool IsDeleted { get; set; }
}
