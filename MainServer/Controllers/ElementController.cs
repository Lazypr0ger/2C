using Contracts.AdapterContracts;
using Contracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MainServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ElementController(IElementAdapterContract elementAdapter,
    ILogger<ElementController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllElement()
    {
        return elementAdapter.GetAllElement().GetResponse(Request,Response);
    }

    [HttpGet("elementId/{elementId}")]
    public IActionResult GetElement(string elementId)
    {
        return elementAdapter.GetElementById(elementId).GetResponse(Request,Response);
    }

    [HttpPost]
    public IActionResult CreateElement([FromBody] ElementVM element)
    {
        return elementAdapter.CreateElement(element).GetResponse(Request, Response);
    }

    [HttpPut]
    public IActionResult UpdateElement([FromBody] ElementVM element)
    {
        return elementAdapter.UpdateElement(element).GetResponse(Request, Response);
    }

    [HttpDelete]
    public IActionResult DeleteElement(string elementId)
    {
        return elementAdapter.DeleteElement(elementId).GetResponse(Request, Response);
    }

    [HttpPatch("{elementId}")]
    public IActionResult RecoveryElement(string elementId)
    {
        return elementAdapter.RecoveryElement(elementId).GetResponse(Request, Response);
    }

    [HttpPut("calculateTotalElementCost/{element}")]
    public IActionResult CalculateElementCost(int count, decimal costRealisation)
    {
        return elementAdapter.CalculateTotalCostElement(count, costRealisation).GetResponse(Request, Response);
    }

}
