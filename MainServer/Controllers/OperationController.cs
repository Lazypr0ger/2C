using Contracts.AdapterContracts;
using Contracts.BindingModels;
using Microsoft.AspNetCore.Mvc;

namespace MainServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OperationController(IOperationAdapterContract adapter) : ControllerBase
{
    // GET api/Operation?from=2026-01-01&to=2026-01-31
    [HttpGet]
    public IActionResult GetAll([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => adapter.GetList(from, to).GetResponse(Request, Response);

    [HttpGet("id/{id}")]
    public IActionResult GetById(string id)
        => adapter.GetElement(id).GetResponse(Request, Response);

    [HttpPost]
    public IActionResult Create([FromBody] OperationBM bm)
        => adapter.Create(bm).GetResponse(Request, Response);

    [HttpPut]
    public IActionResult Update([FromBody] OperationBM bm)
        => adapter.Update(bm).GetResponse(Request, Response);

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
        => adapter.Delete(id).GetResponse(Request, Response);

    [HttpPatch("{id}")]
    public IActionResult Recovery(string id)
        => adapter.Recovery(id).GetResponse(Request, Response);
}
