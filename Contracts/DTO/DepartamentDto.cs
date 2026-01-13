namespace Contracts.DTO;

public class DepartamentDto(string id, string name, string chartOfAccountId,string depChartNum, bool isDeleted)
{

    public string Id { get; set; } = id;

    public string Name { get; set; } = name;

    public string ChartOfAccountId { get; set; } = chartOfAccountId;
    public string DepChartNum { get; set; } = depChartNum;

    //public List<ProductionDto>? Production { get; set; } = production;

    public bool IsDeleted { get; set; } = isDeleted;
}
