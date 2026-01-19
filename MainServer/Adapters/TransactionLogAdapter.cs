using AutoMapper;
using Contracts.AdapterContracts;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.ViewModels;

namespace MainServer.Adapters;

public class TransactionLogAdapter(
    ITransactionLogBusinessLogic bl,
    ILogger<TransactionLogAdapter> logger,
    IMapper mapper) : ITransactionLogAdapterContract
{
    public TransactionLogOperationResponse GetList(DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var list = bl.GetAll(from, to).Select(mapper.Map<TransactionLogVM>).ToList();
            return TransactionLogOperationResponse.OK(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return TransactionLogOperationResponse.InternalServerError(ex.Message);
        }
    }

    public TransactionLogOperationResponse GetElement(string id)
    {
        try
        {
            return TransactionLogOperationResponse.OK(mapper.Map<TransactionLogVM>(bl.GetById(id)));
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return TransactionLogOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            logger.LogError(ex, "ElementNotFoundException");
            return TransactionLogOperationResponse.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return TransactionLogOperationResponse.InternalServerError(ex.Message);
        }
    }

    public TransactionLogOperationResponse GetByOperationId(string operationId)
    {
        try
        {
            var list = bl.GetByOperationId(operationId).Select(mapper.Map<TransactionLogVM>).ToList();
            return TransactionLogOperationResponse.OK(list);
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return TransactionLogOperationResponse.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return TransactionLogOperationResponse.InternalServerError(ex.Message);
        }
    }

    public TransactionLogOperationResponse Create(TransactionLogBM bm)
    {
        try
        {
            bl.Create(mapper.Map<TransactionLogDto>(bm));
            return TransactionLogOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return TransactionLogOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            logger.LogError(ex, "StorageException");
            return TransactionLogOperationResponse.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return TransactionLogOperationResponse.InternalServerError(ex.Message);
        }
    }

    public TransactionLogOperationResponse Update(TransactionLogBM bm)
    {
        try
        {
            bl.Update(mapper.Map<TransactionLogDto>(bm));
            return TransactionLogOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return TransactionLogOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            logger.LogError(ex, "ElementNotFoundException");
            return TransactionLogOperationResponse.NotFound(ex.Message);
        }
        catch (StorageException ex)
        {
            logger.LogError(ex, "StorageException");
            return TransactionLogOperationResponse.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return TransactionLogOperationResponse.InternalServerError(ex.Message);
        }
    }

    public TransactionLogOperationResponse Delete(string id)
    {
        try
        {
            bl.Delete(id);
            return TransactionLogOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return TransactionLogOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            logger.LogError(ex, "ElementNotFoundException");
            return TransactionLogOperationResponse.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return TransactionLogOperationResponse.InternalServerError(ex.Message);
        }
    }

    public TransactionLogOperationResponse Recovery(string id)
    {
        try
        {
            bl.Recovery(id);
            return TransactionLogOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return TransactionLogOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            logger.LogError(ex, "ElementNotFoundException");
            return TransactionLogOperationResponse.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return TransactionLogOperationResponse.InternalServerError(ex.Message);
        }
    }
}
