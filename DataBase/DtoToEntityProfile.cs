using AutoMapper;
using Contracts.DTO;
using Contracts.DTO.HistoriesDto;
using DataBase.Entities;
using DataBase.Entities.HistoriesModel;

namespace DataBase;

public class DtoToEntityProfile : Profile
{
    public DtoToEntityProfile()
    {
        CreateMap<ChartOfAccountDto, ChartOfAccount>().ReverseMap();

        CreateMap<DepartamentDto, Departament>().ReverseMap();
        CreateMap<DepartamentHistoryDto, DepartamentHistory>().ReverseMap();

        CreateMap<ProductionDto, Production>().ReverseMap();
        CreateMap<ProductionHistoryDto, ProductionHistory>().ReverseMap();

        CreateMap<OrganisationDto, Organisation>().ReverseMap();
        CreateMap<OrganisationHistoryDto, OrganisationHistory>().ReverseMap();

        CreateMap<OperationDto, Operation>()
            .ForMember(d => d.Element, o => o.MapFrom(s => s.Elements))
            .ReverseMap()
            .ForMember(d => d.Elements, o => o.MapFrom(s => s.Element));
       
        CreateMap<ElementDto, Element>()
            .ForMember(d => d.Id, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.Id)))
            .ForMember(d => d.Operation, o => o.Ignore())
            .ForMember(d => d.Production, o => o.Ignore());

        CreateMap<Element, ElementDto>();

        CreateMap<TransactionLogDto, TransactionLog>().ReverseMap();
    }
}
