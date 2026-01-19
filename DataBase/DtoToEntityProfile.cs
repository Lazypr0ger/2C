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

        CreateMap<OperationDto, Operation>().ReverseMap();
        CreateMap<ElementDto, Element>().ReverseMap();
        CreateMap<TransactionLogDto, TransactionLog>().ReverseMap();
    }
}
