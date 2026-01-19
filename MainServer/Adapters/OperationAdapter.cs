using AutoMapper;
using Contracts.AdapterContracts;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.ViewModels;

namespace MainServer.Adapters;

public class OperationAdapter(
    IOperationBusinessLogic bl,
    ILogger<OperationAdapter> logger,
    IMapper mapper) : IOperationAdapterContract
{
    public OperationOperationResponse GetList(DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var list = bl.GetAll(from, to).Select(mapper.Map<OperationVM>).ToList();
            return OperationOperationResponse.OK(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return OperationOperationResponse.InternalServerError(ex.Message);
        }
    }

    public OperationOperationResponse GetElement(string id)
    {
        try
        {
            return OperationOperationResponse.OK(mapper.Map<OperationVM>(bl.GetById(id)));
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return OperationOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            logger.LogError(ex, "ElementNotFoundException");
            return OperationOperationResponse.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return OperationOperationResponse.InternalServerError(ex.Message);
        }
    }

    public OperationOperationResponse Create(OperationBM bm)
    {
        try
        {
            bl.Create(mapper.Map<OperationDto>(bm));
            return OperationOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            logger.LogError(ex, "ArgumentNullException");
            return OperationOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return OperationOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            logger.LogError(ex, "StorageException");
            return OperationOperationResponse.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return OperationOperationResponse.InternalServerError(ex.Message);
        }
    }

    public OperationOperationResponse Update(OperationBM bm)
    {
        try
        {
            bl.Update(mapper.Map<OperationDto>(bm));
            return OperationOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return OperationOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            logger.LogError(ex, "ElementNotFoundException");
            return OperationOperationResponse.NotFound(ex.Message);
        }
        catch (StorageException ex)
        {
            logger.LogError(ex, "StorageException");
            return OperationOperationResponse.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return OperationOperationResponse.InternalServerError(ex.Message);
        }
    }

    public OperationOperationResponse Delete(string id)
    {
        try
        {
            bl.Delete(id);
            return OperationOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return OperationOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            logger.LogError(ex, "ElementNotFoundException");
            return OperationOperationResponse.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return OperationOperationResponse.InternalServerError(ex.Message);
        }
    }

    public OperationOperationResponse Recovery(string id)
    {
        try
        {
            bl.Recovery(id);
            return OperationOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return OperationOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            logger.LogError(ex, "ElementNotFoundException");
            return OperationOperationResponse.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return OperationOperationResponse.InternalServerError(ex.Message);
        }
    }
}
