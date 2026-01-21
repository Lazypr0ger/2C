using Contracts.Enums;

namespace Contracts.DTO.Reports;

public class ReportResultDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public ReportTypeCodes TypeCode { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime From { get; set; }
    public DateTime To { get; set; }

    public DateTime BuildDate { get; set; } = DateTime.UtcNow;

    // Для отчёта 1 (может быть null для остальных)
    public decimal? TotalActualCosts { get; set; }

    // Общий итог количества (для отчётов 1 и 3, для 2 можно оставлять 0)
    public decimal TotalQty { get; set; }

    // Универсальные итоги по таблице (3 денежные колонки)
    public decimal Total1 { get; set; }
    public decimal Total2 { get; set; }
    public decimal Total3 { get; set; }

    // Строки (в зависимости от TypeCode заполняется одна коллекция)
    public List<ActualCostDistributionRowDto> ActualCostDistributionRows { get; set; } = new();
    public List<SalesStatementRowDto> SalesStatementRows { get; set; } = new();
    public List<RealisedDeviationRowDto> RealisedDeviationRows { get; set; } = new();
}

public class ActualCostDistributionRowDto
{
    public string ProductionId { get; set; } = string.Empty;
    public string ProductionCode { get; set; } = string.Empty;
    public string ProductionName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public decimal PlanCost { get; set; }
    public decimal Deviation { get; set; }
    public decimal ActualCost { get; set; }
}

public class SalesStatementRowDto
{
    public string ProductionId { get; set; } = string.Empty;
    public string ProductionCode { get; set; } = string.Empty;
    public string ProductionName { get; set; } = string.Empty;

    public decimal SoldAmount { get; set; }
    public decimal SalesCost { get; set; }
    public decimal ProfitOrLoss { get; set; }
}

public class RealisedDeviationRowDto
{
    public string ProductionId { get; set; } = string.Empty;
    public string ProductionCode { get; set; } = string.Empty;
    public string ProductionName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public decimal PlanSalesCost { get; set; }
    public decimal Deviation { get; set; }
    public decimal ActualSalesCost { get; set; }
}
