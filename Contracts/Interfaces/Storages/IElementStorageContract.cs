using Contracts.DTO;

namespace Contracts.Interfaces.Storages;

public interface IElementStorageContract
{
    List<ElementDto> GetAll();
    ElementDto GetById(int id);
    void Create(ElementDto elementDto);
    void Update(ElementDto elementDto);
    void Delete(int id);
}
