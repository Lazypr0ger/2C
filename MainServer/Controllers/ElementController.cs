using Contracts.AdapterContracts;
using Contracts.BindingModels;
using Microsoft.AspNetCore.Mvc;

namespace MainServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ElementController(IElementAdapterContract adapter) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
        => adapter.GetList().GetResponse(Request, Response);

    [HttpGet("id/{id}")]
    public IActionResult GetById(string id)
        => adapter.GetElement(id).GetResponse(Request, Response);

    [HttpGet("operationId/{operationId}")]
    public IActionResult GetByOperationId(string operationId)
        => adapter.GetByOperationId(operationId).GetResponse(Request, Response);

    [HttpPost]
    public IActionResult Create([FromBody] ElementBM bm)
        => adapter.Create(bm).GetResponse(Request, Response);

    [HttpPut]
    public IActionResult Update([FromBody] ElementBM bm)
        => adapter.Update(bm).GetResponse(Request, Response);

    [HttpPatch("{id}")]
    public IActionResult Recovery(string id)
        => adapter.Recovery(id).GetResponse(Request, Response);

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
        => adapter.Delete(id).GetResponse(Request, Response);
}
