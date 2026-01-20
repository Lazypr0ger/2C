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

        CreateMap<Operation, OperationDto>()
            .ForMember(d => d.Elements, o => o.MapFrom(s => s.Element));

        CreateMap<OperationDto, Operation>();

        CreateMap<Element, ElementDto>();
        CreateMap<ElementDto, Element>();

        CreateMap<TransactionLogDto, TransactionLog>().ReverseMap();
    }
}
