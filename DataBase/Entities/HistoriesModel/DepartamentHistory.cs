using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.ViewModels;

namespace DataBase.Entities.HistoriesModel;

public class DepartamentHistory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string DepartamentId { get; set; }

    public string? OldName { get; set; } 

    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    [ForeignKey("DepartamentId")]
    public Departament? Departament { get; set; }
}
