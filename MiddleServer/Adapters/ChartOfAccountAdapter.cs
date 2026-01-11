using AutoMapper;
using Contracts.AdapterContracts;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.ViewModels;

namespace MiddleServer.Adapters;

public class ChartOfAccountAdapter : IChartOfAccountAdapterContract
{
    private readonly IChartOfAccountBusinessLogic _chartOfAccountbusinessLogic;
    private readonly ILogger<ChartOfAccountAdapter> _logger;
    private readonly IMapper _mapper;


    public ChartOfAccountAdapter(IChartOfAccountBusinessLogic chartOfAccountbusinessLogic, ILogger<ChartOfAccountAdapter> logger, IMapper mapper)
    {
        _chartOfAccountbusinessLogic = chartOfAccountbusinessLogic;
        _logger =  logger;
        _mapper = mapper;
    }

    public ChartOfAccountOperationResponse CreateChart(ChartOfAccountVM chrtmodel)
    {
        try
        {
            var data = _mapper.Map<ChartOfAccountDto>(chrtmodel);
            _chartOfAccountbusinessLogic.Create(_mapper.Map<ChartOfAccountDto>(chrtmodel));
            return  ChartOfAccountOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ChartOfAccountOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ChartOfAccountOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ChartOfAccountOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ChartOfAccountOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ChartOfAccountOperationResponse GetChartByName(string name)
    {
        throw new NotImplementedException();
    }

    public ChartOfAccountOperationResponse GetChartByNum(string num)
    {
        throw new NotImplementedException();
    }

    public ChartOfAccountOperationResponse GetElement(string id)
    {
        throw new NotImplementedException();
    }

    public ChartOfAccountOperationResponse GetList()
    {
        try
        {
            return ChartOfAccountOperationResponse.OK([.. _chartOfAccountbusinessLogic.GetAll().Select(x => _mapper.Map<ChartOfAccountVM>(x))]);
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return ChartOfAccountOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ChartOfAccountOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ChartOfAccountOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ChartOfAccountOperationResponse MarkDeleteChart(string id)
    {
        throw new NotImplementedException();
    }

    public ChartOfAccountOperationResponse UpdateChart(ChartOfAccountVM chrtmodel)
    {
        throw new NotImplementedException();
    }
}
