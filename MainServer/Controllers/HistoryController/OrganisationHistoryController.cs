using Contracts.AdapterContracts.HistoryIAdapterContracts;
using Microsoft.AspNetCore.Mvc;

namespace MainServer.Controllers.HistoryController;

[ApiController]
[Route("api/[controller]")]
public class OrganisationHistoryController(IOrganisationHistoryAdapterContract adapter) : ControllerBase
{
    [HttpGet("{organisationId}")]
    public IActionResult GetHistory(string organisationId)
        => adapter.GetHistory(organisationId).GetResponse(Request, Response);

    [HttpGet("{organisationId}/asof")]
    public IActionResult GetAsOf(string organisationId, [FromQuery] DateTime atUtc)
        => adapter.GetAsOf(organisationId, atUtc).GetResponse(Request, Response);

    [HttpPatch("restore/{historyId}")]
    public IActionResult RestoreFromHistory(string historyId)
        => adapter.RestoreFromHistory(historyId).GetResponse(Request, Response);
}
