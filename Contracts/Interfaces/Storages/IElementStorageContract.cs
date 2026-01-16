using Contracts.DTO;

namespace Contracts.Interfaces.Storages;

public interface IElementStorageContract
{
    List<ElementDto> GetAll();
    ElementDto GetById(string id);

    ElementDto GetByOrder(int id);

    List<ElementDto> GetAllByOperation(int id);
    void Create(ElementDto elementDto);
    void Update(ElementDto elementDto);
    void Delete(string id);

}
