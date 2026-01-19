using AutoMapper;
using Contracts.AdapterContracts.HistoryIAdapterContracts;
using Contracts.AdapterContracts.OperationResponses.HistoryOperationResponcse;
using Contracts.Exceptions;
using Contracts.Interfaces.Business.HistoryBusinessLogicContracts;
using Contracts.ViewModels;
using Contracts.ViewModels.HistoryModels;

namespace MainServer.Adapters.HistoryAdapters;

public class DepartamentHistoryAdapter : IDepartamentHistoryAdapterContract
{
    private readonly IDepartamentHistoryBusinessLogic _bl;
    private readonly IMapper _mapper;
    private readonly ILogger<DepartamentHistoryAdapter> _logger;

    public DepartamentHistoryAdapter(
        IDepartamentHistoryBusinessLogic bl,
        IMapper mapper,
        ILogger<DepartamentHistoryAdapter> logger)
    {
        _bl = bl;
        _mapper = mapper;
        _logger = logger;
    }

    public DepartamentHistoryOperationResponse GetAsOf(string departamentId, DateTime atUtc)
    {
        try
        {
            var dto = _bl.GetAsOf(departamentId, atUtc);
            return DepartamentHistoryOperationResponse.OK(_mapper.Map<DepartamentVM>(dto));
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return DepartamentHistoryOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return DepartamentHistoryOperationResponse.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepartamentHistoryOperationResponse.InternalServerError(ex.Message);
        }
    }

    public DepartamentHistoryOperationResponse GetHistory(string departamentId)
    {
        try
        {
            var list = _bl.GetHistory(departamentId);
            var vm = list.Select(x => _mapper.Map<DepartamentHistoryVM>(x)).ToList();
            return DepartamentHistoryOperationResponse.OK(vm);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return DepartamentHistoryOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepartamentHistoryOperationResponse.InternalServerError(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepartamentHistoryOperationResponse.InternalServerError(ex.Message);
        }
    }
    public DepartamentHistoryOperationResponse RestoreFromHistory(string historyId)
    {
        try
        {
            _bl.RestoreFromHistory(historyId);
            return DepartamentHistoryOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return DepartamentHistoryOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return DepartamentHistoryOperationResponse.NotFound(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepartamentHistoryOperationResponse.InternalServerError(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepartamentHistoryOperationResponse.InternalServerError(ex.Message);
        }
    }

}
