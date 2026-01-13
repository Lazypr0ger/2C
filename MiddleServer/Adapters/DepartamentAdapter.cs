using AutoMapper;
using BusinessLogic;
using Contracts.AdapterContracts;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.ViewModels;
using DataBase.Entities;

namespace MiddleServer.Adapters;

public class DepartamentAdapter : IDepartamentAdapterContract
{
    private readonly IDepartamentBusinessLogic _departamentBusinessLogic;
    private readonly ILogger<DepartamentAdapter> _logger;
    private readonly IMapper _mapper;
    public DepartamentAdapter(IDepartamentBusinessLogic departamentBusinessLogic, ILogger<DepartamentAdapter> logger, IMapper mapper)
    {
        _departamentBusinessLogic = departamentBusinessLogic;
        _logger = logger;
        _mapper = mapper;
    }

    public DepartamentOperationResponse CreateDepartament(DepartamentVM departament)
    {
        try
        {
            _logger.LogInformation("МЫ ЗАШЛИ В АДАПТЕР");
            //todo mapper fuck
            //var dto = new DepartamentDto(departament.Id, departament.Name, departament.ChartOfAccountId, departament.DepChartNum, departament.Production
              // .Select(x => new ProductionDto(x.Id, x.Code,x.TypeProduct, x.Name, x.PlannedCost, x.IsDeleted)).ToList(), departament.IsDeleted);
            _departamentBusinessLogic.Create(_mapper.Map<DepartamentDto>(departament));
           // _departamentBusinessLogic.Create(dto));
            return DepartamentOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return DepartamentOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return DepartamentOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepartamentOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepartamentOperationResponse.InternalServerError(ex.Message);
        }
    }

    public DepartamentOperationResponse DeleteDepartament(string id)
    {
        try
        {
            _departamentBusinessLogic.Delete(id);
            return DepartamentOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return DepartamentOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return DepartamentOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepartamentOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepartamentOperationResponse.InternalServerError(ex.Message);
        }
    }

    public DepartamentOperationResponse GetDepartamentByName(string name)
    {
        try
        {
            return DepartamentOperationResponse.OK(_mapper.Map<DepartamentVM>(_departamentBusinessLogic.GetByName(name)));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return DepartamentOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return DepartamentOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepartamentOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepartamentOperationResponse.InternalServerError(ex.Message);
        }
    }

    public DepartamentOperationResponse GetDepartamentListByChartNum(string num)
    {
        try
        {
            return DepartamentOperationResponse.OK(_mapper.Map<DepartamentVM>(_departamentBusinessLogic.GetDepartamentsByChart(num)));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return DepartamentOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return DepartamentOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepartamentOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepartamentOperationResponse.InternalServerError(ex.Message);
        }
    }

    public DepartamentOperationResponse GetDepartamentProductionListById(string id)
    {
        throw new NotImplementedException();
    }

    public DepartamentOperationResponse GetElement(string id)
    {
        try
        { 
            return DepartamentOperationResponse.OK(_mapper.Map<DepartamentVM>(_departamentBusinessLogic.GetById(id)));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return DepartamentOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return DepartamentOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepartamentOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepartamentOperationResponse.InternalServerError(ex.Message);
        }
    }

    public DepartamentOperationResponse GetList()
    {
        try
        {
            return DepartamentOperationResponse.OK([.. _departamentBusinessLogic.GetAll().Select(x => _mapper.Map<DepartamentVM>(x))]);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return DepartamentOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return DepartamentOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepartamentOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepartamentOperationResponse.InternalServerError(ex.Message);
        }
    }

    public DepartamentOperationResponse UpdateDepartament(DepartamentVM departament)
    {
        try
        {
            _departamentBusinessLogic.Update(_mapper.Map<DepartamentDto>(departament));
            return DepartamentOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return DepartamentOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return DepartamentOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return DepartamentOperationResponse.BadRequest($"Not found element by Id {departament.Id}");
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return DepartamentOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepartamentOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepartamentOperationResponse.InternalServerError(ex.Message);
        }
    }
}
