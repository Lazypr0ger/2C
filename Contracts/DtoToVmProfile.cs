
using AutoMapper;
using Contracts.DTO;
using Contracts.ViewModels;

namespace Contracts;

public class DtoToVmProfile : Profile
{
    public DtoToVmProfile()
    {
        CreateMap<ChartOfAccountVM,ChartOfAccountDto>().ReverseMap();
        CreateMap<DepartamentDto, DepartamentVM>().ReverseMap();
        CreateMap<ProductionDto, ProductionVM>().ReverseMap();
        //CreateMap<ElementDto, ElementVM>().ReverseMap();
        //CreateMap<OperationDto, Operation>().ReverseMap();
        //CreateMap<OrganisationDto, Organisation>().ReverseMap();
        //CreateMap<TransactionLogDto, TransactionLog>().ReverseMap();
    }

}
