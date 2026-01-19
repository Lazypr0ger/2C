using AutoMapper;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Implementation;

public class ElementStorageContract(TwoCDbContext db, IMapper mapper) : IElementStorageContract
{
    private readonly TwoCDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public void Create(ElementDto dto)
    {
        try
        {
            var entity = _mapper.Map<Operation>(dto);

            // ВАЖНО: не даём EF вставлять строки/проводки вместе с шапкой
            entity.Element = new();
            entity.TransactionLog = new();

            _db.Operations.Add(entity);
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void Update(ElementDto dto)
    {
        try
        {
            var entity = GetEntity(dto.Id) ?? throw new ElementNotFoundException(dto.Id ?? "null");
            _db.Elements.Update(_mapper.Map(dto, entity));
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            if (ex is ElementNotFoundException) throw;
            throw new StorageException(ex);
        }
    }

    public void Delete(string id)
    {
        try
        {
            var entity = GetEntity(id) ?? throw new ElementNotFoundException(id);
            entity.IsDeleted = true;
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            if (ex is ElementNotFoundException) throw;
            throw new StorageException(ex);
        }
    }

    public void Recovery(string id)
    {
        try
        {
            var entity = GetEntity(id) ?? throw new ElementNotFoundException(id);
            entity.IsDeleted = false;
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            if (ex is ElementNotFoundException) throw;
            throw new StorageException(ex);
        }
    }

    public List<ElementDto> GetAll()
    {
        try
        {
            var list = _db.Elements.AsNoTracking().ToList();
            return list.Select(_mapper.Map<ElementDto>).ToList();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ElementDto GetById(string id)
    {
        try
        {
            var entity = _db.Elements.AsNoTracking().FirstOrDefault(x => x.Id == id);
            return _mapper.Map<ElementDto>(entity);
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public List<ElementDto> GetByOperationId(string operationId)
    {
        try
        {
            var list = _db.Elements
                .AsNoTracking()
                .Where(x => x.OperationId == operationId)
                .ToList();

            return list.Select(_mapper.Map<ElementDto>).ToList();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    private Element? GetEntity(string? id)
        => string.IsNullOrWhiteSpace(id) ? null : _db.Elements.FirstOrDefault(x => x.Id == id);
}
