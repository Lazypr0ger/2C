using Contracts.AdapterContracts;
using Contracts.AdapterContracts.HistoryIAdapterContracts;
using Microsoft.AspNetCore.Mvc;

namespace MainServer.Controllers.HistoryController;

[ApiController]
[Route("api/[controller]")]
public class DepartamentHistoryController : ControllerBase
{
    private readonly IDepartamentHistoryAdapterContract _adapter;

    public DepartamentHistoryController(IDepartamentHistoryAdapterContract adapter)
    {
        _adapter = adapter;
    }

    // GET /api/DepartamentHistory/{departamentId}
    [HttpGet("{departamentId}")]
    public IActionResult GetHistory(string departamentId)
        => _adapter.GetHistory(departamentId).GetResponse(Request, Response);

    [HttpGet("{departamentId}/asof")]
    public IActionResult GetAsOf(string departamentId, [FromQuery] DateTime atUtc)
    => _adapter.GetAsOf(departamentId, atUtc).GetResponse(Request, Response);

    // PATCH /api/DepartamentHistory/restore/{historyId}
    [HttpPatch("restore/{historyId}")]
    public IActionResult RestoreFromHistory(string historyId)
        => _adapter.RestoreFromHistory(historyId).GetResponse(Request, Response);

}
