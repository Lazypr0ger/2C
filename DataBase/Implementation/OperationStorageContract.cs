using Contracts.DTO;
using Contracts.Enums;
using Contracts.Interfaces.Storages;

namespace DataBase.Implementation;

public class OperationStorageContract : IOperationStorageContract
{
    public void Create(OperationDto operationDto)
    {
        throw new NotImplementedException();
    }

    public void Delete(string Id)
    {
        throw new NotImplementedException();
    }

    public List<OperationDto> GetAllOperationByDepartamentId(TypeDocument typeDocument, string departamentId)
    {
        throw new NotImplementedException();
    }

    public List<OperationDto> GetAllOperationByDepartamentIdByDate(TypeDocument typeDocument, string departamentId, DateTime startDate, DateTime endTime)
    {
        throw new NotImplementedException();
    }

    public List<OperationDto> GetAllOperationByOrganisationId(TypeDocument typeDocument, string organisationId)
    {
        throw new NotImplementedException();
    }

    public List<OperationDto> GetAllOperationByOrganisationIdByDate(TypeDocument typeDocument, string organisationId, DateTime startDate, DateTime endTime)
    {
        throw new NotImplementedException();
    }

    public List<OperationDto> GetAllOperationDocument(TypeDocument typeDocument)
    {
        throw new NotImplementedException();
    }

    public OperationDto GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public OperationDto GetByName(string name)
    {
        throw new NotImplementedException();
    }

    public List<OperationDto> GetOperationByDate(TypeDocument typeDocument, DateTime startDate, DateTime endTime)
    {
        throw new NotImplementedException();
    }

    public void Update(OperationDto operationDto)
    {
        throw new NotImplementedException();
    }
}
