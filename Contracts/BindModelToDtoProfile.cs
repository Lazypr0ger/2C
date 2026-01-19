using AutoMapper;
using Contracts.BindingModels;
using Contracts.DTO;

namespace Contracts
{
    public class BindModelToDtoProfile : Profile
    {
        public BindModelToDtoProfile() 
        {
            CreateMap<DepartamentBM, DepartamentDto>().ReverseMap();
            CreateMap<OrganisationBM, OrganisationDto>().ReverseMap();
            CreateMap<ProductionBM, ProductionDto>().ReverseMap();
        }
    }
}
