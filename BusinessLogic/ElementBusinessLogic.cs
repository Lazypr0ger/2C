using Contracts.DTO;
using Contracts.Interfaces.Business;
using Contracts.Interfaces.Storages;
using Microsoft.Extensions.Logging;

namespace BusinessLogic;

public class ElementBusinessLogic(IElementStorageContract elementStorage, ILogger<ElementBusinessLogic> logger) : IElementBusinessLogic
{
    public decimal CalculateTotalCostElement(int countElement, decimal costRealisation)
    {
        return countElement * costRealisation;
    }

    public void Create(ElementDto elementDto)
    {
        elementStorage.Create(elementDto);
    }

    public void Delete(string id)
    {
        elementStorage.Delete(id);
    }

    public List<ElementDto> GetAll()
    {
        return elementStorage.GetAll();
    }

    public ElementDto GetById(string id)
    {
        return elementStorage.GetById(id);
    }

    public void Recovery(string id)
    {
       elementStorage.Recovery(id);
    }

    public void Update(ElementDto elementDto)
    {
        elementStorage.Update(elementDto);
    }
}
