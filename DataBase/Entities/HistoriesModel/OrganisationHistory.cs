using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Entities.HistoriesModel
{
    public class OrganisationHistory
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string OrganisationId { get; set; }
        public string? Name { get; set; }

        public string? AccountNumOrg {get; set;}

        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        [ForeignKey ("OrganisationId")]

        public Organisation? Organisation { get; set; }

        public bool IsDeleted { get; set; }
    }
}
