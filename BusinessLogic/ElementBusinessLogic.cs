using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class ElementBusinessLogic(
    IElementStorageContract storage,
    ILogger<ElementBusinessLogic> logger) : IElementBusinessLogic
{
    public List<ElementDto> GetAll()
    {
        var list = storage.GetAll();
        return list ?? throw new NullListException();
    }

    public ElementDto GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Element id is empty");

        return storage.GetById(id) ?? throw new ElementNotFoundException(id);
    }

    public List<ElementDto> GetByOperationId(string operationId)
    {
        if (string.IsNullOrWhiteSpace(operationId))
            throw new ValidationException("OperationId is empty");

        var list = storage.GetByOperationId(operationId);
        return list ?? throw new NullListException();
    }

    public void Create(ElementDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            dto.Id = Guid.NewGuid().ToString();

        if (string.IsNullOrWhiteSpace(dto.OperationId))
            throw new ValidationException("OperationId is empty");

        if (string.IsNullOrWhiteSpace(dto.ProductionId))
            throw new ValidationException("ProductionId is empty");

        if (dto.CountElement <= 0)
            throw new ValidationException("CountElement must be > 0");

        dto.IsDeleted = false;

        logger.LogInformation("Creating element. OpId={OpId}, ProdId={ProdId}, Count={Count}",
            dto.OperationId, dto.ProductionId, dto.CountElement);

        storage.Create(dto);
    }

    public void Update(ElementDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ValidationException("Element id is empty");

        if (string.IsNullOrWhiteSpace(dto.OperationId))
            throw new ValidationException("OperationId is empty");

        if (string.IsNullOrWhiteSpace(dto.ProductionId))
            throw new ValidationException("ProductionId is empty");

        if (dto.CountElement <= 0)
            throw new ValidationException("CountElement must be > 0");

        logger.LogInformation("Updating element. Id={Id}", dto.Id);

        storage.Update(dto);
    }

    public void Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Element id is empty");

        logger.LogInformation("Deleting element. Id={Id}", id);
        storage.Delete(id);
    }

    public void Recovery(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("Element id is empty");

        logger.LogInformation("Recovery element. Id={Id}", id);
        storage.Recovery(id);
    }
}
