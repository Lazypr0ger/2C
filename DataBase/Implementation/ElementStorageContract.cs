
using Contracts.DTO;
using Contracts.Interfaces.Storages;

namespace DataBase.Implementation;

public class ElementStorageContract : IElementStorageContract
{
    public decimal CalculateTotalCostElement(int countElement, decimal RealisationCost)
    {
        throw new NotImplementedException();
    }

    public void Create(ElementDto elementDto)
    {
        throw new NotImplementedException();
    }

    public void Delete(string id)
    {
        throw new NotImplementedException();
    }

    public List<ElementDto> GetAll()
    {
        throw new NotImplementedException();
    }

    public List<ElementDto> GetAllByOperation(int id)
    {
        throw new NotImplementedException();
    }

    public ElementDto GetById(string id)
    {
        throw new NotImplementedException();
    }

    public ElementDto GetByOrder(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(ElementDto elementDto)
    {
        throw new NotImplementedException();
    }
}
