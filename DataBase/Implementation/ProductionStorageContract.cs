using Contracts.DTO;
using Contracts.Interfaces.Storages;

namespace DataBase.Implementation;

public class ProductionStorageContract : IProductionStorageContract
{
    public void Create(ProductionDto productionDto)
    {
        throw new NotImplementedException();
    }

    public void Delete(string id)
    {
        throw new NotImplementedException();
    }

    public List<ProductionDto> GetAll()
    {
        throw new NotImplementedException();
    }

    public ProductionDto GetByCode(string Code)
    {
        throw new NotImplementedException();
    }

    public ProductionDto GetById(string Id)
    {
        throw new NotImplementedException();
    }

    public ProductionDto GetByName(string Name)
    {
        throw new NotImplementedException();
    }

    public List<ProductionDto> GetByType()
    {
        throw new NotImplementedException();
    }

    public void Update(ProductionDto productionDto)
    {
        throw new NotImplementedException();
    }
}
