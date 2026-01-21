using Contracts.AdapterContracts;
using Contracts.BindingModels;
using Microsoft.AspNetCore.Mvc;

namespace MainServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController(IReportAdapterContract reportAdapter, ILogger<ReportController> logger) : ControllerBase
{
    [HttpPost("build")]
    public IActionResult Build([FromBody] ReportBuildBM bm)
    {
        return reportAdapter.Build(bm).GetResponse(Request, Response);
    }
}
