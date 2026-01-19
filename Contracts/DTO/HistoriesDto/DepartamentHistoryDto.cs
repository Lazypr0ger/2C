
namespace Contracts.DTO.HistoriesDto
{
    public class DepartamentHistoryDto(string departamentid, string oldName)
    {
        public string DepartamentId { get; set; } = departamentid;

        public string? Name { get; set; } = oldName;

        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public bool IsDeleted { get; set; }
    }
}
