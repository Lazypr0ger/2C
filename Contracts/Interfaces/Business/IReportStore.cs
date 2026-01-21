using Contracts.DTO.Reports;
using Contracts.Enums;

namespace Contracts.Interfaces.Business;

public interface IReportStore
{
    void Save(ReportResultDto report);
    ReportResultDto GetById(string id);
    List<ReportListItemDto> GetList(ReportTypeCodes? typeCode = null);

    void Delete(string id);
}
