using Contracts.AdapterContracts;
using Contracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MiddleServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionController(IProductionAdapterContract productionAdapter, ILogger<ProductionController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllProduction()
    {
        return productionAdapter.GetAll().GetResponse(Request,Response);
    }

    [HttpGet("id/{id}")]
    public IActionResult GetProductionById(string id)
    {
        return productionAdapter.GetById(id).GetResponse(Request,Response);
    }

    [HttpGet("name/{name}")]
    public IActionResult GetProductByName(string name)
    {
        return productionAdapter.GetByName(name).GetResponse(Request,Response);
    }

    [HttpGet("code/{code}")]
    public IActionResult GetProductByCode(string code)
    {
        return productionAdapter.GetByCode(code).GetResponse(Request, Response);
    }

    [HttpGet("{departamentName}/products")]
    public IActionResult GetListProductsByDepartament(string departamentName)
    {
        return productionAdapter.GetProductsByDepartament(departamentName).GetResponse(Request, Response);
    }

    [HttpPost]
    public IActionResult CreateProduct([FromBody] ProductionVM production)
    {
        return productionAdapter.Create(production).GetResponse(Request,Response);
    }

    [HttpPut]
    public IActionResult UpdateProduct([FromBody] ProductionVM production)
    {
        return productionAdapter.Update(production).GetResponse(Request,Response);
    }

    [HttpDelete]
    public IActionResult DeleteProduct(string id)
    {
        return productionAdapter.Delete(id).GetResponse(Request,Response);
    }

    [HttpPatch("{id}")]
    public IActionResult RecoveryProduct(string id)
    {
        return productionAdapter.RecoveryProduct(id).GetResponse(Request,Response);
    }
}
