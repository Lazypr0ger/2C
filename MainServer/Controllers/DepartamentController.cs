using Contracts.AdapterContracts;
using Contracts.BindingModels;
using Contracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MainServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartamentController(IDepartamentAdapterContract adapter) : ControllerBase
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
        return adapter.GetDepartamentByName(name).GetResponse(Request, Response);
    }

    [HttpPost]
    public IActionResult Register([FromBody] DepartamentBM model)
    {
        return adapter.CreateDepartament(model).GetResponse(Request, Response);
    }

    [HttpPut]
    public IActionResult ChangeInfo([FromBody] DepartamentBM model)
    {
        return adapter.UpdateDepartament(model).GetResponse(Request, Response);
    }

    [HttpPatch("{id}")]
    public IActionResult Restore(string id)
    {
        return adapter.RecoveryDepartament(id).GetResponse(Request, Response);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        return adapter.DeleteDepartament(id).GetResponse(Request, Response);
    }


}
