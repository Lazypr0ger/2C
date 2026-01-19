using AutoMapper;
using BusinessLogic;
using Contracts.AdapterContracts;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.ViewModels;
using DataBase.Entities;

namespace MainServer.Adapters;

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

    public DepartamentOperationResponse CreateDepartament(DepartamentBM departament)
    {
        try
        {
            _departamentBusinessLogic.Create(_mapper.Map<DepartamentDto>(departament));
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
            var getAll = _departamentBusinessLogic.GetAll();
            var result = getAll.Select(x => _mapper.Map<DepartamentVM>(x));
            return DepartamentOperationResponse.OK([.. result]);
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

    public DepartamentOperationResponse RecoveryDepartament(string id)
    {
        try
        {
            _departamentBusinessLogic.Recovery(id);
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
            return DepartamentOperationResponse.BadRequest($"Not found element by Id {id}");
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

    public DepartamentOperationResponse UpdateDepartament(DepartamentBM departament)
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
