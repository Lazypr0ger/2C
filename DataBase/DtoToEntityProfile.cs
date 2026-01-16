using AutoMapper;
using Contracts.DTO;
using DataBase.Entities;

namespace DataBase;

public class DtoToEntityProfile : Profile
{
    public DtoToEntityProfile() 
    {
        CreateMap<ChartOfAccountDto, ChartOfAccount>().ReverseMap();
        CreateMap<Departament, DepartamentDto>().ReverseMap();
        CreateMap<ProductionDto, Production>().ReverseMap();
        CreateMap<ElementDto, Element>().ReverseMap();
        CreateMap<OperationDto, Operation>().ReverseMap();
        CreateMap<Organisation,OrganisationDto >().ReverseMap();
        CreateMap<TransactionLogDto, TransactionLog>().ReverseMap();
    }
}
