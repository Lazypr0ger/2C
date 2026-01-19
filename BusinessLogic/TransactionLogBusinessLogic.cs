using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class TransactionLogBusinessLogic(
    ITransactionLogStorageContract storage,
    ILogger<TransactionLogBusinessLogic> logger) : ITransactionLogBusinessLogic
{
    public List<TransactionLogDto> GetAll(DateTime? from = null, DateTime? to = null)
    {
        var list = storage.GetAll(from, to);
        return list ?? throw new NullListException();
    }
    public List<TransactionLogDto> GetView(DateTime? from = null, DateTime? to = null)
        => storage.GetView(from, to) ?? throw new NullListException();
    public TransactionLogDto GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("TransactionLog id is empty");

        return storage.GetById(id) ?? throw new ElementNotFoundException(id);
    }

    public List<TransactionLogDto> GetByOperationId(string operationId)
    {
        if (string.IsNullOrWhiteSpace(operationId))
            throw new ValidationException("OperationId is empty");

        var list = storage.GetByOperationId(operationId);
        return list ?? throw new NullListException();
    }

    public void Create(TransactionLogDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        dto.Id ??= Guid.NewGuid().ToString();

        if (dto.DateOperation == default)
            dto.DateOperation = DateTime.UtcNow;

        if (string.IsNullOrWhiteSpace(dto.ChartOfAccountDebId))
            throw new ValidationException("ChartOfAccountDebId is empty");

        if (string.IsNullOrWhiteSpace(dto.ChartOfAccountCredId))
            throw new ValidationException("ChartOfAccountCredId is empty");

        if (dto.Amount == 0m)
            throw new ValidationException("Amount must be not 0");

        dto.IsDeleted = false;

        logger.LogInformation("Create posting. Id={Id}, Deb={Deb}, Cred={Cred}, Amount={Amount}",
            dto.Id, dto.ChartOfAccountDebId, dto.ChartOfAccountCredId, dto.Amount);

        storage.Create(dto);
    }

    public void Update(TransactionLogDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ValidationException("TransactionLog id is empty");

        if (string.IsNullOrWhiteSpace(dto.ChartOfAccountDebId))
            throw new ValidationException("ChartOfAccountDebId is empty");

        if (string.IsNullOrWhiteSpace(dto.ChartOfAccountCredId))
            throw new ValidationException("ChartOfAccountCredId is empty");

        if (dto.Amount == 0m)
            throw new ValidationException("Amount must be not 0");

        logger.LogInformation("Update posting. Id={Id}", dto.Id);

        storage.Update(dto);
    }

    public void Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("TransactionLog id is empty");

        logger.LogInformation("Delete posting. Id={Id}", id);
        storage.Delete(id);
    }

    public void Recovery(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ValidationException("TransactionLog id is empty");

        logger.LogInformation("Recovery posting. Id={Id}", id);
        storage.Recovery(id);
    }
}
