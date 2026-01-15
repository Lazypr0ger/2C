namespace Contracts.DTO.HistoriesDto;

public class OrganisationHistoryDto(string organisationId, string oldName,
    string oldAccountNum)
{
    public string? OrganisationId { get; set; } = organisationId;
    public string? OldName { get; set; } = oldName;

    public string? OldAccountNumOrg {get; set;} = oldAccountNum;

    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}
