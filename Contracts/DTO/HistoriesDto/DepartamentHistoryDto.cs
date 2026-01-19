
namespace Contracts.DTO.HistoriesDto
{
    public class DepartamentHistoryDto
    {
        public string? Id { get; set; }
        public string? DepartamentId { get; set; }

        public string? Name { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public bool IsDeleted { get; set; }
    }
}
