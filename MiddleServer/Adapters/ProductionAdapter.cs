using AutoMapper;
using BusinessLogic;
using Contracts.AdapterContracts;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.DTO;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.ViewModels;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MiddleServer.Adapters;

public class ProductionAdapter(IProductionBusinessLogic productionBusinessLogic,
    ILogger<ProductionAdapter> logger, IMapper mapper) : IProductionAdapterContract
{
    private readonly IProductionBusinessLogic _productionBusinessLogic = productionBusinessLogic;
    private readonly ILogger<ProductionAdapter> _logger = logger;
    private readonly IMapper _mapper = mapper;

    public ProductionOperationResponse Create(ProductionVM production)
    {
        try
        {
            productionBusinessLogic.Create(_mapper.Map<ProductionDto>(production));
            return ProductionOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ProductionOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ProductionOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ProductionOperationResponse Delete(string id)
    {
        try
        {
            productionBusinessLogic.Delete(id);
            return ProductionOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ProductionOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ProductionOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ProductionOperationResponse GetAll()
    {
        try
        {
            return ProductionOperationResponse.OK([.. productionBusinessLogic.GetAll().Select(x => _mapper.Map<ProductionVM>(x))]);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ProductionOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ProductionOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ProductionOperationResponse GetByCode(string code)
    {
        try
        {
            return ProductionOperationResponse.OK(_mapper.Map<ProductionVM>(productionBusinessLogic.GetByCode(code))); 
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ProductionOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ProductionOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ProductionOperationResponse GetById(string id)
    {
        try
        {
            return ProductionOperationResponse.OK(_mapper.Map<ProductionVM>(productionBusinessLogic.GetById(id)));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ProductionOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ProductionOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ProductionOperationResponse GetByName(string name)
    {
        try
        {
            return ProductionOperationResponse.OK(_mapper.Map<ProductionVM>(productionBusinessLogic.GetByName(name)));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ProductionOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ProductionOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ProductionOperationResponse GetByType(TypeProduct product)
    {
        try
        {
            return ProductionOperationResponse.OK([.. productionBusinessLogic.GetByType(product).Select(x => _mapper.Map<ProductionVM>(x))]);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ProductionOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ProductionOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ProductionOperationResponse GetProductsByDepartament(string departametnName)
    {
        try
        {
            return ProductionOperationResponse.OK([.. productionBusinessLogic.GetproductByDepartamentName(departametnName).Select(x => _mapper.Map<ProductionVM>(x))]);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ProductionOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ProductionOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ProductionOperationResponse RecoveryProduct(string id)
    {
        try
        {
            productionBusinessLogic.Recovery(id);
            return ProductionOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ProductionOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ProductionOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ProductionOperationResponse Update(ProductionVM production)
    {
        try
        {
            productionBusinessLogic.Update(_mapper.Map<ProductionDto>(production));
            return ProductionOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ProductionOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ProductionOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ProductionOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ProductionOperationResponse.InternalServerError(ex.Message);
        }
    }
}
