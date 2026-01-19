using AutoMapper;
using Contracts.AdapterContracts;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.ViewModels;

namespace MainServer.Adapters;

public class ElementAdapter(
    IElementBusinessLogic bl,
    ILogger<ElementAdapter> logger,
    IMapper mapper) : IElementAdapterContract
{
    public ElementOperationResponse GetList()
    {
        try
        {
            var list = bl.GetAll().Select(mapper.Map<ElementVM>).ToList();
            return ElementOperationResponse.OK(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return ElementOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ElementOperationResponse GetElement(string id)
    {
        try
        {
            return ElementOperationResponse.OK(mapper.Map<ElementVM>(bl.GetById(id)));
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return ElementOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            logger.LogError(ex, "ElementNotFoundException");
            return ElementOperationResponse.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return ElementOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ElementOperationResponse GetByOperationId(string operationId)
    {
        try
        {
            var list = bl.GetByOperationId(operationId).Select(mapper.Map<ElementVM>).ToList();
            return ElementOperationResponse.OK(list);
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return ElementOperationResponse.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return ElementOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ElementOperationResponse Create(ElementBM bm)
    {
        try
        {
            bl.Create(mapper.Map<ElementDto>(bm));
            return ElementOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return ElementOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            logger.LogError(ex, "StorageException");
            return ElementOperationResponse.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return ElementOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ElementOperationResponse Update(ElementBM bm)
    {
        try
        {
            bl.Update(mapper.Map<ElementDto>(bm));
            return ElementOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return ElementOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            logger.LogError(ex, "ElementNotFoundException");
            return ElementOperationResponse.NotFound(ex.Message);
        }
        catch (StorageException ex)
        {
            logger.LogError(ex, "StorageException");
            return ElementOperationResponse.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return ElementOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ElementOperationResponse Recovery(string id)
    {
        try
        {
            bl.Recovery(id);
            return ElementOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return ElementOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            logger.LogError(ex, "ElementNotFoundException");
            return ElementOperationResponse.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return ElementOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ElementOperationResponse Delete(string id)
    {
        try
        {
            bl.Delete(id);
            return ElementOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return ElementOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            logger.LogError(ex, "ElementNotFoundException");
            return ElementOperationResponse.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return ElementOperationResponse.InternalServerError(ex.Message);
        }
    }
}
