namespace Contracts.DTO;

public class OperationSaveDto
{
    public required OperationDto Operation { get; set; }
    public List<ElementDto> Elements { get; set; } = new();
    public List<TransactionLogDto> Postings { get; set; } = new();
}
