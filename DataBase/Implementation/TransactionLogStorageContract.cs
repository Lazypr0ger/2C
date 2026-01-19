using AutoMapper;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Implementation;

public class TransactionLogStorageContract(TwoCDbContext db, IMapper mapper) : ITransactionLogStorageContract
{
    private readonly TwoCDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public List<TransactionLogDto> GetAll(DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var q = _db.TransactionLogs.AsNoTracking().AsQueryable();

            if (from.HasValue) q = q.Where(x => x.DateOperation >= from.Value);
            if (to.HasValue) q = q.Where(x => x.DateOperation <= to.Value);

            var list = q.OrderByDescending(x => x.DateOperation).ToList();
            return list.Select(_mapper.Map<TransactionLogDto>).ToList();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public TransactionLogDto GetById(string id)
    {
        try
        {
            var entity = _db.TransactionLogs.AsNoTracking().FirstOrDefault(x => x.Id == id);
            return _mapper.Map<TransactionLogDto>(entity);
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public List<TransactionLogDto> GetByOperationId(string operationId)
    {
        try
        {
            var list = _db.TransactionLogs
                .AsNoTracking()
                .Where(x => x.OperationId == operationId)
                .OrderBy(x => x.DateOperation)
                .ToList();

            return list.Select(_mapper.Map<TransactionLogDto>).ToList();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void Create(TransactionLogDto dto)
    {
        try
        {
            _db.TransactionLogs.Add(_mapper.Map<TransactionLog>(dto));
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void Update(TransactionLogDto dto)
    {
        try
        {
            var entity = GetEntity(dto.Id) ?? throw new ElementNotFoundException(dto.Id ?? "null");
            _db.TransactionLogs.Update(_mapper.Map(dto, entity));
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

    private TransactionLog? GetEntity(string? id)
        => string.IsNullOrWhiteSpace(id) ? null : _db.TransactionLogs.FirstOrDefault(x => x.Id == id);
}
