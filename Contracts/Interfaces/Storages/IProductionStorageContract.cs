using Contracts.DTO;
using Contracts.Enums;

namespace Contracts.Interfaces.Storages;

public interface IProductionStorageContract
{
    List<ProductionDto> GetAll();
    List<ProductionDto> GetByType(TypeProduct typeProduct);

    List<ProductionDto> GetProductByDepartamentName(string departamentName);
    ProductionDto GetById(string Id);
    ProductionDto GetByName(string Name);
    ProductionDto GetByCode(string Code);

    void Create(ProductionDto productionDto);
    void Update(ProductionDto productionDto);

    void Recovery(string id);
    void Delete(string id);

}
