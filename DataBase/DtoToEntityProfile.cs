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
        CreateMap<Departament, DepartamentDto>().ReverseMap();
        CreateMap<DepartamentHistory, DepartamentHistoryDto>().ReverseMap();
        CreateMap<ProductionDto, Production>().ReverseMap();
        CreateMap<ProductionHistory, ProductionHistoryDto>().ReverseMap();
        CreateMap<ElementDto, Element>().ReverseMap();
        CreateMap<OperationDto, Operation>().ReverseMap();
        CreateMap<Organisation,OrganisationDto >().ReverseMap();
        CreateMap<OrganisationHistory, OrganisationHistoryDto>().ReverseMap();
        CreateMap<TransactionLogDto, TransactionLog>().ReverseMap();
    }
}
