using AutoMapper;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Implementation;

public class OperationStorageContract(TwoCDbContext db, IMapper mapper) : IOperationStorageContract
{
    private readonly TwoCDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public List<OperationDto> GetAll(DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var q = _db.Operations
                .AsNoTracking()
                .Include(x => x.Element)
                .AsQueryable();

            if (from.HasValue) q = q.Where(x => x.DateOperation >= from.Value);
            if (to.HasValue) q = q.Where(x => x.DateOperation <= to.Value);

            var list = q.OrderByDescending(x => x.DateOperation).ToList();
            return list.Select(_mapper.Map<OperationDto>).ToList();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public OperationDto GetById(string id)
    {
        try
        {
            var entity = _db.Operations
                .AsNoTracking()
                .Include(x => x.Element)
                .FirstOrDefault(x => x.Id == id);

            return _mapper.Map<OperationDto>(entity);
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void Create(OperationDto dto)
    {
        try
        {
            _db.Operations.Add(_mapper.Map<Operation>(dto));
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void Update(OperationDto dto)
    {
        try
        {
            var entity = GetEntity(dto.Id) ?? throw new ElementNotFoundException(dto.Id ?? "null");
            _db.Operations.Update(_mapper.Map(dto, entity));
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

    public void ReplaceElements(string operationId, List<ElementDto> elements)
    {
        try
        {
            var old = _db.Elements.Where(x => x.OperationId == operationId).ToList();
            _db.Elements.RemoveRange(old);

            foreach (var e in elements)
            {
                e.Id ??= Guid.NewGuid().ToString();
                e.OperationId = operationId;
                _db.Elements.Add(_mapper.Map<Element>(e));
            }

            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void ReplaceTransactionLogs(string operationId, List<TransactionLogDto> logs)
    {
        try
        {
            var old = _db.TransactionLogs.Where(x => x.OperationId == operationId).ToList();
            _db.TransactionLogs.RemoveRange(old);

            foreach (var l in logs)
            {
                l.Id ??= Guid.NewGuid().ToString();
                l.OperationId = operationId;
                _db.TransactionLogs.Add(_mapper.Map<TransactionLog>(l));
            }

            _db.SaveChanges();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public Dictionary<string, string> GetAccountIdsByNums(IEnumerable<string> nums)
    {
        try
        {
            return _db.ChartOfAccounts
                .AsNoTracking()
                .Where(x => nums.Contains(x.NumChart))
                .ToDictionary(x => x.NumChart, x => x.Id);
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public Dictionary<string, decimal> GetPlannedCostsByProductIds(IEnumerable<string> productIds)
    {
        try
        {
            return _db.Productions
                .AsNoTracking()
                .Where(x => productIds.Contains(x.Id))
                .ToDictionary(x => x.Id, x => x.PlannedCost ?? 0m);
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    private Operation? GetEntity(string? id)
        => string.IsNullOrWhiteSpace(id) ? null : _db.Operations.FirstOrDefault(x => x.Id == id);
}
