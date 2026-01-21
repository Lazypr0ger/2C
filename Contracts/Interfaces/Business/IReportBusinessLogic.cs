using Contracts.DTO.Reports;

namespace Contracts.Interfaces.Business;

public interface IReportBusinessLogic
{
    // Строит отчёт и возвращает готовые строки + итоги
    ReportResultDto Build(ReportBuildRequestDto request);
}
