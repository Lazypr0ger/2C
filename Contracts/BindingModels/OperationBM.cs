using Contracts.Enums;

namespace Contracts.BindingModels;

public class OperationBM
{
    public string? Id { get; set; }            // null для Create
    public string? NameDocument { get; set; }
    public DateTime? DateOperation { get; set; }  // null -> сервер поставит UtcNow
    public OperationType Type { get; set; }

    public string? Comment { get; set; }       // <-- ВАЖНО

    public string? OrganisationId { get; set; }
    public string? DepartamentId { get; set; }

    public List<ElementBM> Elements { get; set; } = new();
}
