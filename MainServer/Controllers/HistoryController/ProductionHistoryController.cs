using Contracts.AdapterContracts.HistoryIAdapterContracts;
using Microsoft.AspNetCore.Mvc;

namespace MainServer.Controllers.HistoryController;

[ApiController]
[Route("api/[controller]")]
public class ProductionHistoryController(IProductionHistoryAdapterContract adapter) : ControllerBase
{
    [HttpGet("{productionId}")]
    public IActionResult GetHistory(string productionId)
        => adapter.GetHistory(productionId).GetResponse(Request, Response);

    [HttpGet("{productionId}/asof")]
    public IActionResult GetAsOf(string productionId, [FromQuery] DateTime atUtc)
        => adapter.GetAsOf(productionId, atUtc).GetResponse(Request, Response);

    [HttpPatch("restore/{historyId}")]
    public IActionResult RestoreFromHistory(string historyId)
        => adapter.RestoreFromHistory(historyId).GetResponse(Request, Response);
}
