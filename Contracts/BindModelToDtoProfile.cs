using AutoMapper;
using Contracts.BindingModels;
using Contracts.DTO;
using Contracts.ViewModels;

namespace Contracts
{
    public class BindModelToDtoProfile : Profile
    {
        public BindModelToDtoProfile() 
        {
            CreateMap<DepartamentBM, DepartamentDto>().ReverseMap();
            CreateMap<OrganisationBM, OrganisationDto>().ReverseMap();
            CreateMap<ProductionBM, ProductionDto>().ReverseMap();
            CreateMap<OperationBM, OperationDto>().ReverseMap();
            CreateMap<ElementBM, ElementDto>().ReverseMap();
            CreateMap<TransactionLogBM, TransactionLogDto>().ReverseMap();
        }
    }
}
