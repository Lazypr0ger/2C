using Contracts.AdapterContracts;
using Contracts.BindingModels;
using Contracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MainServer.Controllers;

[ApiController]
[Route("api/[controller]")]

public class OrganisationController(IOrganisationAdapterContract adapter) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllOrganisation()
    {
        return adapter.GetAll().GetResponse(Request, Response);
    }

    [HttpGet("id/{id}")]
    public IActionResult GetOrganisationById(string id)
    {
        return adapter.GetById(id).GetResponse(Request, Response);
    }

    [HttpGet("name/{name}")]
    public IActionResult GetOrganisationByname(string name)
    {
        return adapter.GetByName(name).GetResponse(Request, Response);
    }

    [HttpPost]
    public IActionResult RegisterOrganisation([FromBody] OrganisationBM model)
    {
        return adapter.Create(model).GetResponse(Request, Response);
    }

    [HttpPut]
    public IActionResult ChangeOrganisation([FromBody] OrganisationBM model)
    {
        return adapter.Update(model).GetResponse(Request, Response);
    }
    
    [HttpPatch("{id}")]
    public IActionResult Restore(string id)
    {
        return adapter.RecoveryOrganisation(id).GetResponse(Request, Response);
    }

    [HttpDelete("{id}")]
    public IActionResult RemoveOrganisation(string id)
    {
        return adapter.Delete(id).GetResponse(Request, Response);
    }
}
