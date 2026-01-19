using AutoMapper;
using Contracts.AdapterContracts.HistoryIAdapterContracts;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.AdapterContracts.OperationResponses.HistoryOperationResponcse;
using Contracts.Exceptions;
using Contracts.Interfaces.Business.HistoryBusinessLogicContracts;
using Contracts.ViewModels;

namespace MainServer.Adapters.HistoryAdapters;

public class OrganisationHistoryAdapter : IOrganisationHistoryAdapterContract
{
    private readonly IOrganisationHistoryBusinessLogic _bl;
    private readonly IMapper _mapper;
    private readonly ILogger<OrganisationHistoryAdapter> _logger;

    public OrganisationHistoryAdapter(
        IOrganisationHistoryBusinessLogic bl,
        IMapper mapper,
        ILogger<OrganisationHistoryAdapter> logger)
    {
        _bl = bl;
        _mapper = mapper;
        _logger = logger;
    }

    public OrganisationHistoryOperationResponse GetHistory(string organisationId)
    {
        try
        {
            var dto = _bl.GetHistory(organisationId);
            var vm = dto.Select(x => _mapper.Map<Contracts.ViewModels.HistoryModels.OrganisationHistoryVM>(x)).ToList();
            return OrganisationHistoryOperationResponse.OK(vm);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return OrganisationHistoryOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return OrganisationHistoryOperationResponse.InternalServerError(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return OrganisationHistoryOperationResponse.InternalServerError(ex.Message);
        }
    }

    public OrganisationHistoryOperationResponse GetAsOf(string organisationId, DateTime atUtc)
    {
        try
        {
            var dto = _bl.GetAsOf(organisationId, atUtc);
            return OrganisationHistoryOperationResponse.OK(_mapper.Map<OrganisationVM>(dto));
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return OrganisationHistoryOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return OrganisationHistoryOperationResponse.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return OrganisationHistoryOperationResponse.InternalServerError(ex.Message);
        }
    }

    public OrganisationHistoryOperationResponse RestoreFromHistory(string historyId)
    {
        try
        {
            _bl.RestoreFromHistory(historyId);
            return OrganisationHistoryOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return OrganisationHistoryOperationResponse.BadRequest(ex.Message);
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return OrganisationHistoryOperationResponse.NotFound(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return OrganisationHistoryOperationResponse.InternalServerError(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return OrganisationHistoryOperationResponse.InternalServerError(ex.Message);
        }
    }
}
