using AutoMapper;
using Contracts.AdapterContracts;
using Contracts.AdapterContracts.OperationResponses;
using Contracts.BindingModels;
using Contracts.DTO.Reports;
using Contracts.Exceptions;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;     
using Contracts.ViewModels.Reports;

namespace MainServer.Adapters;

public class ReportAdapter(
    IReportBusinessLogic bl,
    IReportStore reportStore,           
    ILogger<ReportAdapter> logger,
    IMapper mapper) : IReportAdapterContract
{
    public ReportOperationResponse Build(ReportBuildBM bm)
    {
        try
        {
            if (bm is null) return ReportOperationResponse.BadRequest("Data is empty");

            var dto = mapper.Map<ReportBuildRequestDto>(bm);


            var resultDto = bl.Build(dto);

            reportStore.Save(resultDto);

            return ReportOperationResponse.OK(mapper.Map<ReportResultVM>(resultDto));
        }
        catch (ArgumentNullException ex)
        {
            logger.LogError(ex, "ArgumentNullException");
            return ReportOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return ReportOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            logger.LogError(ex, "StorageException");
            return ReportOperationResponse.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return ReportOperationResponse.InternalServerError(ex.Message);
        }
    }
    public ReportOperationResponse Delete(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                return ReportOperationResponse.BadRequest("Id is empty");

            bl.Delete(id);
            return ReportOperationResponse.NoContent();
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "ValidationException");
            return ReportOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            logger.LogError(ex, "StorageException");
            return ReportOperationResponse.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception");
            return ReportOperationResponse.InternalServerError(ex.Message);
        }
    }
}
