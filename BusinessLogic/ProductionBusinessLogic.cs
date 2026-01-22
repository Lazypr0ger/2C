using Contracts.DTO;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class ProductionBusinessLogic(
    IProductionStorageContract productStorage,
    IDepartamentStorageContract departamentStorage,
    ILogger<ProductionBusinessLogic> logger) : IProductionBusinessLogic
{
    public void Create(ProductionDto productionDto)
    {
        if (productionDto is null)
            throw new ArgumentNullException(nameof(productionDto));

        // обязательные поля
        if (string.IsNullOrWhiteSpace(productionDto.Code))
            throw new ValidationException("Product code is empty");

        if (string.IsNullOrWhiteSpace(productionDto.Name))
            throw new ValidationException("Product name is empty");

        if (string.IsNullOrWhiteSpace(productionDto.DepartamentId))
            throw new ValidationException("DepartamentId is empty");

        // бизнес-правила
        if (productionDto.Type == TypeProduct.None)
            throw new ValidationException("Product type is not selected");

        if (productionDto.PlannedCost < 0)
            throw new ValidationException("PlannedCost cannot be negative");

        //  Id генерируем на сервере
        if (string.IsNullOrWhiteSpace(productionDto.Id))
            productionDto.Id = Guid.NewGuid().ToString();

        //  при создании всегда активный
        productionDto.IsDeleted = false;

        logger.LogInformation("Creating product. Code={Code}, Name={Name}, Type={Type}, Id={Id}",
            productionDto.Code, productionDto.Name, productionDto.Type, productionDto.Id);

        productStorage.Create(productionDto);
    }

    public void Update(ProductionDto productionDto)
    {
        if (productionDto is null)
            throw new ArgumentNullException(nameof(productionDto));

        if (string.IsNullOrWhiteSpace(productionDto.Id))
            throw new ValidationException("Product id is empty");

        if (string.IsNullOrWhiteSpace(productionDto.Code))
            throw new ValidationException("Product code is empty");

        if (string.IsNullOrWhiteSpace(productionDto.Name))
            throw new ValidationException("Product name is empty");

        if (string.IsNullOrWhiteSpace(productionDto.DepartamentId))
            throw new ValidationException("DepartamentId is empty");

        if (productionDto.Type == TypeProduct.None)
            throw new ValidationException("Product type is not selected");

        if (productionDto.PlannedCost < 0)
            throw new ValidationException("PlannedCost cannot be negative");

        logger.LogInformation("Updating product. Id={Id}", productionDto.Id);
        productStorage.Update(productionDto);
    }

    public void Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Product id is empty");

        logger.LogInformation("Deleting product. Id={Id}", id);
        productStorage.Delete(id);
    }

    public void Recovery(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Product id is empty");

        logger.LogInformation("Recovery product. Id={Id}", id);
        productStorage.Recovery(id);
    }

    public List<ProductionDto> GetAll()
        => productStorage.GetAll() ?? throw new NullListException();

    public ProductionDto GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Product id is empty");

        return productStorage.GetById(id)
               ?? throw new ElementNotFoundException($"Product with Id {id} not found");
    }

    public ProductionDto GetByCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ValidationException("Product code is empty");

        return productStorage.GetByCode(code)
               ?? throw new ElementNotFoundException($"Product with code {code} not found");
    }

    public ProductionDto GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Product name is empty");

        return productStorage.GetByName(name)
               ?? throw new ElementNotFoundException($"Product with name {name} not found");
    }

    public List<ProductionDto> GetByType(TypeProduct typeProduct)
    {
        if (typeProduct == TypeProduct.None)
            throw new ValidationException("Product type is not selected");

        return productStorage.GetByType(typeProduct) ?? throw new NullListException();
    }

    public List<ProductionDto> GetproductByDepartamentName(string departamentName)
    {
        if (string.IsNullOrWhiteSpace(departamentName))
            throw new ValidationException("Departament name is empty");

        // гарантируем что департамент существует
        var dep = departamentStorage.GetByName(departamentName)
                  ?? throw new ElementNotFoundException($"Departament with name {departamentName} not found");

        // storage ожидает имя департамента
        return productStorage.GetProductByDepartamentName(dep.Name) ?? throw new NullListException();
    }
}
