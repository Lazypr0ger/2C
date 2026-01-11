using System.Xml.Linq;
using AutoMapper;
using Contracts.DTO;
using Contracts.ViewModels;

namespace Contracts;

public class DtoToVmProfile : Profile
{
    public DtoToVmProfile() 
    {
        CreateMap<ChartOfAccountDto, ChartOfAccountVM>().ReverseMap();
        //CreateMap<DepartamentDto, Departament>().ReverseMap();
        //CreateMap<ProductionDto, Production>().ReverseMap();
        //CreateMap<ElementDto, Element>().ReverseMap();
        //CreateMap<OperationDto, Operation>().ReverseMap();
        //CreateMap<OrganisationDto, Organisation>().ReverseMap();
        //CreateMap<TransactionLogDto, TransactionLog>().ReverseMap();
    }

}
