using Contracts.AdapterContracts;
using Contracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MiddleServer.Adapters;

namespace MiddleServer.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ChartOfAccountController(IChartOfAccountAdapterContract adapter) : ControllerBase
{
    private readonly IChartOfAccountAdapterContract _adapter = adapter;
    [HttpGet]
    public IActionResult GetAllRecords()
    {
        return _adapter.GetList().GetResponse(Request,Response);
    }

    [HttpPost]
    public IActionResult Register([FromBody] ChartOfAccountVM model)
    {
        return _adapter.CreateChart(model).GetResponse(Request, Response);
    }
}
