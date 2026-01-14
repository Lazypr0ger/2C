using System.Xml.Linq;
using AutoMapper;
using BusinessLogic;
using Contracts.AdapterContracts;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.ViewModels;

namespace MiddleServer.Adapters;

public class OrganisationAdapter(IOrganisationBusinessLogic organisationBusinessLogic, ILogger<OrganisationAdapter> logger, IMapper mapper) : IOrganisationAdapterContract
{
    private readonly IOrganisationBusinessLogic _organisationBusinessLogic = organisationBusinessLogic;
    private readonly ILogger<OrganisationAdapter> _logger = logger;
    private readonly IMapper _mapper = mapper;

    public OrganisationOperationResponse Create(OrganisationVM organisation)
    {
        try
        {
            organisationBusinessLogic.Create(_mapper.Map<OrganisationDto>(organisation));
            return OrganisationOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return OrganisationOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return OrganisationOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return OrganisationOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return OrganisationOperationResponse.InternalServerError(ex.Message);
        }
    }

    public OrganisationOperationResponse Delete(string id)
    {
        try
        {
            organisationBusinessLogic.Delete(id);
            return OrganisationOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return OrganisationOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return OrganisationOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return OrganisationOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return OrganisationOperationResponse.InternalServerError(ex.Message);
        }
    }

    public OrganisationOperationResponse GetAll()
    {
        try
        {
            return OrganisationOperationResponse.OK([.. organisationBusinessLogic.GetAll().Select(x => _mapper.Map<OrganisationVM>(x))]);
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return OrganisationOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return OrganisationOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return OrganisationOperationResponse.InternalServerError(ex.Message);
        }
    }

    public OrganisationOperationResponse GetById(string id)
    {
        try
        {
            return OrganisationOperationResponse.OK(_mapper.Map<OrganisationVM>(organisationBusinessLogic.GetById(id)));
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return OrganisationOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return OrganisationOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return OrganisationOperationResponse.InternalServerError(ex.Message);
        }
    }

    public OrganisationOperationResponse GetByName(string name)
    {
        try
        {
            return OrganisationOperationResponse.OK(_mapper.Map<OrganisationVM>(organisationBusinessLogic.GetByName(name)));
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return OrganisationOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return OrganisationOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return OrganisationOperationResponse.InternalServerError(ex.Message);
        }
    }

    public OrganisationOperationResponse Update(OrganisationVM organisation )
    {
        try
        {
            organisationBusinessLogic.Update(_mapper.Map<OrganisationDto>(organisation));
            return OrganisationOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return OrganisationOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return OrganisationOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return OrganisationOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return OrganisationOperationResponse.InternalServerError(ex.Message);
        }
    }
}
