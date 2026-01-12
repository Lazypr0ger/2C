using Contracts.AdapterContracts;
using Contracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MiddleServer.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ChartOfAccountController(IChartOfAccountAdapterContract adapter) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllRecords()
    {
        return adapter.GetList().GetResponse(Request, Response);
    }

    [HttpGet("id/{id}")]
    public IActionResult GetById(string id)
    {
        return adapter.GetElement(id).GetResponse(Request, Response);
    }

    [HttpGet("name/{name}")]
    public IActionResult GetByName(string name)
    {
        return adapter.GetChartByName(name).GetResponse(Request, Response);
    }

    [HttpGet("num/{numChat}")]
    public IActionResult GetByNumChat(string numChat)
    {
        return adapter.GetChartByNum(numChat).GetResponse(Request, Response);
    }

    [HttpPost]
    public IActionResult Register([FromBody] ChartOfAccountVM model)
    {
        return adapter.CreateChart(model).GetResponse(Request, Response);
    }
}
