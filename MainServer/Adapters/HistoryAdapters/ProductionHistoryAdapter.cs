using AutoMapper;
using Contracts.AdapterContracts.HistoryIAdapterContracts;
using Contracts.AdapterContracts.OperationResponses.HistoryOperationResponcse;
using Contracts.Exceptions;
using Contracts.Interfaces.Business.HistoryBusinessLogicContracts;
using Contracts.ViewModels;

namespace MainServer.Adapters.HistoryAdapters;

public class ProductionHistoryAdapter : IProductionHistoryAdapterContract
{
    private readonly IProductionHistoryBusinessLogic _bl;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductionHistoryAdapter> _logger;

    public ProductionHistoryAdapter(
        IProductionHistoryBusinessLogic bl,
        IMapper mapper,
        ILogger<ProductionHistoryAdapter> logger)
    {
        _bl = bl;
        _mapper = mapper;
        _logger = logger;
    }

    public ProductionHistoryOperationResponse GetHistory(string productionId)
    {
        try
        {
            var dto = _bl.GetHistory(productionId);
            var vm = dto.Select(x => _mapper.Map<Contracts.ViewModels.HistoryModels.ProductionHistoryVM>(x)).ToList();
            return ProductionHistoryOperationResponse.OK(vm);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionHistoryOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ProductionHistoryOperationResponse.InternalServerError(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionHistoryOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ProductionHistoryOperationResponse GetAsOf(string productionId, DateTime atUtc)
    {
        try
        {
            var dto = _bl.GetAsOf(productionId, atUtc);
            return ProductionHistoryOperationResponse.OK(_mapper.Map<ProductionVM>(dto));
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionHistoryOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return ProductionHistoryOperationResponse.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionHistoryOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ProductionHistoryOperationResponse RestoreFromHistory(string historyId)
    {
        try
        {
            _bl.RestoreFromHistory(historyId);
            return ProductionHistoryOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionHistoryOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return ProductionHistoryOperationResponse.NotFound(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ProductionHistoryOperationResponse.InternalServerError(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionHistoryOperationResponse.InternalServerError(ex.Message);
        }
    }
}
