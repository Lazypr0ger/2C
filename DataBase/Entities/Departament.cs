using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DataBase.Entities.HistoriesModel;
namespace DataBase.Entities;

public class Departament
{
    public required string Id { get; set; }
    public required string Name { get; set; }

    public List<Production>? Productions { get; set; } 

    [DefaultValue(false)]
    public bool IsDeleted { get; set; } = false;

    public List<DepartamentHistory>? DepartamentHistories { get; set; }


}
