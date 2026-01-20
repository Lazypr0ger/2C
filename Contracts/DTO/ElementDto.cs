namespace Contracts.DTO;

public class ElementDto
{
    public string? Id { get; set; }

    public string? OperationId { get; set; }  // ставит сервер при сохранении
    public string? ProductionId { get; set; }

    public int CountElement { get; set; }
    public decimal? Price { get; set; }

    public bool IsDeleted { get; set; }
}
