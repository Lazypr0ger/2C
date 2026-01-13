namespace Contracts.ViewModels;

public class DepartamentVM
{
    public required string Id { get; set; }

    public required string Name { get; set; } 

    public required string ChartOfAccountId { get; set; }
    public required string DepChartNum { get; set; }
    //public List<ProductionVM>? Production { get; set; }
    public bool IsDeleted { get; set; }
}
