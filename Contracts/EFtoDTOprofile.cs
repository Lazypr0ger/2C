using AutoMapper;
using Contracts.DTO;
namespace Contracts;

public class EFtoDTOprofile : Profile
{
    public EFtoDTOprofile()
    {
        CreateMap<ChartOfAccountDto, ChartOfAccount>().ReverseMap;
    }
}
