
using AutoMapper;
using Contracts.DTO;
using Contracts.DTO.HistoriesDto;
using Contracts.DTO.Reports;
using Contracts.ViewModels;
using Contracts.ViewModels.HistoryModels;
using Contracts.ViewModels.Reports;

namespace Contracts;

public class DtoToVmProfile : Profile
{
    public DtoToVmProfile()
    {
        CreateMap<ChartOfAccountVM,ChartOfAccountDto>().ReverseMap();
        CreateMap<DepartamentDto, DepartamentVM>().ReverseMap();
        CreateMap<DepartamentHistoryDto, DepartamentHistoryVM>().ReverseMap();
        CreateMap<ProductionDto, ProductionVM>().ReverseMap();
        CreateMap<ProductionHistoryDto, ProductionHistoryVM>().ReverseMap();
        CreateMap<ElementDto, ElementVM>().ReverseMap();
        CreateMap<OperationDto, OperationVM>().ReverseMap();
        CreateMap<OrganisationDto, OrganisationVM>().ReverseMap();
        CreateMap<OrganisationHistoryDto, OrganisationHistoryVM>().ReverseMap();
        CreateMap<TransactionLogDto, TransactionLogVM>().ReverseMap();


        CreateMap<ReportResultDto, ReportResultVM>().ReverseMap();
        CreateMap<ActualCostDistributionRowDto, ActualCostDistributionRowVM>().ReverseMap();
        CreateMap<SalesStatementRowDto, SalesStatementRowVM>().ReverseMap();
        CreateMap<RealisedDeviationRowDto, RealisedDeviationRowVM>().ReverseMap();
    }

}
