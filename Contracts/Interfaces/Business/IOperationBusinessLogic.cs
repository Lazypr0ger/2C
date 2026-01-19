using Contracts.DTO;

namespace Contracts.Interfaces.Business;

public interface IOperationBusinessLogic
{
    List<OperationDto> GetAll(DateTime? from = null, DateTime? to = null);
    OperationDto GetById(string id);

    void Create(OperationDto dto);
    void Update(OperationDto dto);

    void Delete(string id);
    void Recovery(string id);
}
