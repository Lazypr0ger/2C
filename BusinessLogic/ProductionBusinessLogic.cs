using Contracts.DTO;
using Contracts.Enums;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class ProductionBusinessLogic(IProductionStorageContract productStorage, IDepartamentStorageContract departamentStorage,
    ILogger<ProductionBusinessLogic> logger) : IProductionBusinessLogic
{
    public void Create(ProductionDto productionDto)
    {
        productStorage.Create(productionDto);
    }

    public void Delete(string id)
    {
        productStorage.Delete(id);
    }

    public List<ProductionDto> GetAll()
    {
        return productStorage.GetAll();
    }

    public ProductionDto GetByCode(string Code)
    {
        return productStorage.GetByCode(Code);
    }

    public ProductionDto GetById(string Id)
    {
        return productStorage.GetById(Id);
    }

    public ProductionDto GetByName(string Name)
    {
        return productStorage.GetByName(Name);
    }

    public List<ProductionDto> GetByType(TypeProduct typeProduct)
    {
        return productStorage.GetByType(typeProduct);
    }

    public List<ProductionDto> GetproductByDepartamentName(string departamentName)
    {
       return productStorage.GetProductByDepartamentName(departamentStorage.GetByName(departamentName).Name);
    }

    public void Recovery(string id)
    {
        productStorage.Recovery(id);
    }

    public void Update(ProductionDto productionDto)
    {
       productStorage.Update(productionDto);
    }
}
