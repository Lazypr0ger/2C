using Contracts.DTO;

namespace Contracts.Interfaces.Business;

public interface IProductionBusinessLogic
{    
    List<ProductionDto> GetAll();
    List<ProductionDto> GetByType();
    ProductionDto GetById(string Id);
    ProductionDto GetByName(string Name);
    ProductionDto GetByCode(string Code);

    void Create(ProductionDto productionDto);
    void Update(ProductionDto productionDto);
    void Delete(string id);

}
