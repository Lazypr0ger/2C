using System.Xml.Linq;
using AutoMapper;
using BusinessLogic;
using Contracts.AdapterContracts;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.ViewModels;
using DataBase.Entities;

namespace MainServer.Adapters
{
    public class ElementAdapter : IElementAdapterContract
    {
        IElementBusinessLogic _elementBusinessLogic;
        ILogger<ElementAdapter> _logger;
        IMapper _mapper;

        public ElementOperationResponse CalculateTotalCostElement(int countelement, decimal realisationCost)
        {
            try
            {
               return ElementOperationResponse.OK(_elementBusinessLogic.CalculateTotalCostElement(countelement, realisationCost));

            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException");
                return ElementOperationResponse.BadRequest("Data is empty");
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "ValidationException");
                return ElementOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
            }
            catch (StorageException ex)
            {
                _logger.LogError(ex, "StorageException");
                return ElementOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception");
                return ElementOperationResponse.InternalServerError(ex.Message);
            }
        }

        public ElementOperationResponse CreateElement(ElementVM element)
        {
            try
            {
                _elementBusinessLogic.Create(_mapper.Map<ElementDto>(element));
                return ElementOperationResponse.NoContent();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException");
                return ElementOperationResponse.BadRequest("Data is empty");
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "ValidationException");
                return ElementOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
            }
            catch (StorageException ex)
            {
                _logger.LogError(ex, "StorageException");
                return ElementOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception");
                return ElementOperationResponse.InternalServerError(ex.Message);
            }
        }

        public ElementOperationResponse DeleteElement(string elementId)
        {
            try
            {
                _elementBusinessLogic.Delete(elementId);
                return ElementOperationResponse.NoContent();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException");
                return ElementOperationResponse.BadRequest("Data is empty");
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "ValidationException");
                return ElementOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
            }
            catch (StorageException ex)
            {
                _logger.LogError(ex, "StorageException");
                return ElementOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception");
                return ElementOperationResponse.InternalServerError(ex.Message);
            }
        }

        public ElementOperationResponse GetAllElement()
        {
            try
            {
                var getAll = _elementBusinessLogic.GetAll();
                var result = getAll.Select(x => _mapper.Map<ElementVM>(x));
                return ElementOperationResponse.OK([.. result]);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException");
                return ElementOperationResponse.BadRequest("Data is empty");
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "ValidationException");
                return ElementOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
            }
            catch (StorageException ex)
            {
                _logger.LogError(ex, "StorageException");
                return ElementOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception");
                return ElementOperationResponse.InternalServerError(ex.Message);
            }
        }

        public ElementOperationResponse GetElementById(string elementId)
        {
            try
            {
                return ElementOperationResponse.OK(_mapper.Map<ElementVM>(_elementBusinessLogic.GetById(elementId)));
        
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException");
                return ElementOperationResponse.BadRequest("Data is empty");
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "ValidationException");
                return ElementOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
            }
            catch (StorageException ex)
            {
                _logger.LogError(ex, "StorageException");
                return ElementOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception");
                return ElementOperationResponse.InternalServerError(ex.Message);
            }
        }

        public ElementOperationResponse RecoveryElement(string elementId)
        {
            try
            {
                _elementBusinessLogic.Recovery(elementId);
                return ElementOperationResponse.NoContent();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException");
                return ElementOperationResponse.BadRequest("Data is empty");
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "ValidationException");
                return ElementOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
            }
            catch (StorageException ex)
            {
                _logger.LogError(ex, "StorageException");
                return ElementOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception");
                return ElementOperationResponse.InternalServerError(ex.Message);
            }
        }

        public ElementOperationResponse UpdateElement(ElementVM element)
        {
            try
            {
                _elementBusinessLogic.Update(_mapper.Map<ElementDto>(element));
                return ElementOperationResponse.NoContent();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException");
                return ElementOperationResponse.BadRequest("Data is empty");
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "ValidationException");
                return ElementOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
            }
            catch (StorageException ex)
            {
                _logger.LogError(ex, "StorageException");
                return ElementOperationResponse.BadRequest($"Error while working with data storage: {ex.InnerException!.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception");
                return ElementOperationResponse.InternalServerError(ex.Message);
            }
        }
    }
}
