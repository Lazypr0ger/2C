
using AutoMapper;
using Contracts.DTO;
using Contracts.DTO.HistoriesDto;
using Contracts.ViewModels;
using Contracts.ViewModels.HistoryModels;

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
    }

}
