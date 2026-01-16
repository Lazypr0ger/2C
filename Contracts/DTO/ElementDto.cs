namespace Contracts.DTO;

public class ElementDto(string id, int countProduct, decimal costRealisation, bool isDeleted)
{
    public string Id { get; set; } = id;

    public int CountProduct { get; set; } = countProduct;

    public decimal CostRealisation { get; set; } = costRealisation;
        
    public string? ProductionId { get; set; }
    public string? OperationId { get; set; }

    public bool IsDeleted { get; set; } = isDeleted;
}
