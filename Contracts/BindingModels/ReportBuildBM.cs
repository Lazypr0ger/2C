using Contracts.Enums;

namespace Contracts.BindingModels;

public class ReportBuildBM
{
    public ReportTypeCodes TypeCode { get; set; }

    // Период отчёта
    public DateTime From { get; set; }
    public DateTime To { get; set; }

    // Необязательно: если хочешь сохранять имя/комментарий в списке отчётов
    public string? Name { get; set; }
    public string? Comment { get; set; }
}
