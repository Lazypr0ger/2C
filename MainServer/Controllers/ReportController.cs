using Contracts.AdapterContracts;
using Contracts.Interfaces.Business;
using Contracts.BindingModels;
using Microsoft.AspNetCore.Mvc;
using Contracts.Enums;

namespace MainServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController(
    IReportAdapterContract reportAdapter,
    IReportStore reportStore, IReportBusinessLogic bl,
    ILogger<ReportController> logger) : ControllerBase
{
    [HttpPost("build")]
    public IActionResult Build([FromBody] ReportBuildBM bm)
        => reportAdapter.Build(bm).GetResponse(Request, Response);

    [HttpPost("build-and-save")]
    public IActionResult BuildAndSave([FromBody] ReportBuildBM bm)
    {
        var resp = reportAdapter.Build(bm);

        // resp.Data у тебя лежит внутри OperationResponse (как у остальных)
        // поэтому сохранять правильнее в адаптере. Но можно и тут, если доступ есть.
        // Если у твоего OperationResponse есть GetData<T>(), используй его.

        return resp.GetResponse(Request, Response);
    }

    [HttpGet("list")]
    public IActionResult GetList([FromQuery] ReportTypeCodes? typeCode = null)
           => Ok(bl.GetList(typeCode));

    [HttpGet("id/{id}")]
    public IActionResult GetById(string id)
    {
        var report = reportStore.GetById(id);
        return Ok(report);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
       => reportAdapter.Delete(id).GetResponse(Request, Response);
}
