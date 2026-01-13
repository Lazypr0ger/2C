using AutoMapper;
using Contracts.DTO;
using DataBase.Entities;

namespace DataBase;

public class DtoToEntityProfile : Profile
{
    public DtoToEntityProfile() 
    {
        CreateMap<ChartOfAccountDto, ChartOfAccount>().ReverseMap();
        CreateMap<Departament, DepartamentDto>()
            .ForMember(d => d.DepChartNum, opt => opt.MapFrom(s => s.ChartOfAccount.NumChart));
        CreateMap<DepartamentDto, Departament>()
            .ForMember(d => d.ChartOfAccount, opt => opt.Ignore());
        CreateMap<ProductionDto, Production>().ReverseMap();
        CreateMap<ElementDto, Element>().ReverseMap();
        CreateMap<OperationDto, Operation>().ReverseMap();
        CreateMap<OrganisationDto, Organisation>().ReverseMap();
        CreateMap<TransactionLogDto, TransactionLog>().ReverseMap();
    }
}
