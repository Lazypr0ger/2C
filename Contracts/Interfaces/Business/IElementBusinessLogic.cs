using Contracts.DTO;

namespace Contracts.Interfaces.Business;

public interface IElementBusinessLogic
{
    List<ElementDto> GetAll();
    ElementDto GetById(string id);

    List<ElementDto> GetByOperationId(string operationId);

    void Create(ElementDto dto);
    void Update(ElementDto dto);

    void Recovery(string id);
    void Delete(string id);
}
