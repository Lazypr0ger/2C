using Contracts.Enums;

namespace Contracts.ViewModels.Reports;

public class ReportResultVM
{
    public string Id { get; set; } = string.Empty;

    public ReportTypeCodes TypeCode { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime From { get; set; }
    public DateTime To { get; set; }

    public DateTime BuildDate { get; set; }

    public decimal? TotalActualCosts { get; set; }

    public decimal Total1 { get; set; }
    public decimal Total2 { get; set; }
    public decimal Total3 { get; set; }

    public List<ActualCostDistributionRowVM> ActualCostDistributionRows { get; set; } = new();
    public List<SalesStatementRowVM> SalesStatementRows { get; set; } = new();
    public List<RealisedDeviationRowVM> RealisedDeviationRows { get; set; } = new();
}

public class ActualCostDistributionRowVM
{
    public string ProductionId { get; set; } = string.Empty;
    public string ProductionCode { get; set; } = string.Empty;
    public string ProductionName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public decimal PlanCost { get; set; }
    public decimal Deviation { get; set; }
    public decimal ActualCost { get; set; }
}

public class SalesStatementRowVM
{
    public string ProductionId { get; set; } = string.Empty;
    public string ProductionCode { get; set; } = string.Empty;
    public string ProductionName { get; set; } = string.Empty;

    public decimal SoldAmount { get; set; }
    public decimal SalesCost { get; set; }
    public decimal ProfitOrLoss { get; set; }
}

public class RealisedDeviationRowVM
{
    public string ProductionId { get; set; } = string.Empty;
    public string ProductionCode { get; set; } = string.Empty;
    public string ProductionName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public decimal PlanSalesCost { get; set; }
    public decimal Deviation { get; set; }
    public decimal ActualSalesCost { get; set; }
}
