using Contracts.Enums;

namespace Contracts.BindingModels;

public class OperationBM
{
    public string? Id { get; set; }
    public string? NameDocument { get; set; }

    // важно: клиент должен иметь возможность прислать время
    public DateTime? DateOperation { get; set; }

    public OperationType Type { get; set; }

    public string? Comment { get; set; }
    public decimal? TotalAmountDocument { get; set; }

    public string? OrganisationId { get; set; }
    public string? DepartamentId { get; set; }

    public List<ElementBM> Elements { get; set; } = new();
}
