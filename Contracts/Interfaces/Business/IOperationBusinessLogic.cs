using Contracts.DTO;
using Contracts.Enums;
using System.Diagnostics.SymbolStore;
using System.Dynamic;

namespace Contracts.Interfaces.Business;

public interface IOperationBusinessLogic
{    
    List<OperationDto> GetAllOperationDocument(TypeDocument typeDocument);
    List<OperationDto> GetAllOperationByOrganisationId(TypeDocument typeDocument, string organisationId);
    List<OperationDto> GetAllOperationByDepartamentId(TypeDocument typeDocument, string departamentId);
    List<OperationDto> GetOperationByDate(TypeDocument typeDocument,DateTime startDate, DateTime endTime);
    List<OperationDto> GetAllOperationByOrganisationIdByDate(TypeDocument typeDocument, string organisationId, DateTime startDate, DateTime endTime);
    List<OperationDto> GetAllOperationByDepartamentIdByDate(TypeDocument typeDocument, string departamentId, DateTime startDate, DateTime endTime);
    OperationDto GetById(Guid id);
    OperationDto GetByName(string name);
    
    void Create(OperationDto operationDto);
    void Update(OperationDto operationDto);
    void Delete(string Id);
}
