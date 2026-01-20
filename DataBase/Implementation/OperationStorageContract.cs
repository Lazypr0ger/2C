using AutoMapper;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Implementation;

public class OperationStorageContract : IOperationStorageContract
{
    private readonly TwoCDbContext _db;
    private readonly IMapper _mapper;

    public OperationStorageContract(TwoCDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

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

            var list = q
                .OrderByDescending(x => x.DateOperation)
                .ThenByDescending(x => x.Id)
                .ToList();

            var dtos = list.Select(_mapper.Map<OperationDto>).ToList();

            // Историчность "на дату операции"
            foreach (var dto in dtos)
                FillNamesByHistoryAt(dto);

            return dtos;
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public OperationDto? GetById(string id)
    {
        try
        {
            var entity = _db.Operations
                .AsNoTracking()
                .Include(x => x.Element)
                .FirstOrDefault(x => x.Id == id);

            if (entity == null) return null;

            var dto = _mapper.Map<OperationDto>(entity);

            // Историчность "на дату операции"
            FillNamesByHistoryAt(dto);

            return dto;
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void CreateDocument(OperationDto dto, List<ElementDto> elements, List<TransactionLogDto> logs)
    {
        try
        {
            using var tx = _db.Database.BeginTransaction();

            // 1) шапка
            var opEntity = _mapper.Map<Operation>(dto);
            _db.Operations.Add(opEntity);

            // 2) строки
            foreach (var e in elements ?? new())
            {
                e.Id ??= Guid.NewGuid().ToString();
                e.OperationId = dto.Id;
                _db.Elements.Add(_mapper.Map<Element>(e));
            }

            // 3) проводки
            foreach (var l in logs ?? new())
            {
                l.Id ??= Guid.NewGuid().ToString();
                l.OperationId = dto.Id;
                _db.TransactionLogs.Add(_mapper.Map<TransactionLog>(l));
            }

            _db.SaveChanges();
            tx.Commit();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void UpdateDocument(OperationDto dto, List<ElementDto> elements, List<TransactionLogDto> logs)
    {
        try
        {
            using var tx = _db.Database.BeginTransaction();

            var entity = _db.Operations.FirstOrDefault(x => x.Id == dto.Id);
            if (entity == null) throw new ElementNotFoundException(dto.Id ?? "null");

            // 1) шапка
            _mapper.Map(dto, entity);

            // 2) заменить строки
            var oldEl = _db.Elements.Where(x => x.OperationId == dto.Id).ToList();
            _db.Elements.RemoveRange(oldEl);

            foreach (var e in elements ?? new())
            {
                e.Id ??= Guid.NewGuid().ToString();
                e.OperationId = dto.Id;
                _db.Elements.Add(_mapper.Map<Element>(e));
            }

            // 3) заменить проводки
            var oldLogs = _db.TransactionLogs.Where(x => x.OperationId == dto.Id).ToList();
            _db.TransactionLogs.RemoveRange(oldLogs);

            foreach (var l in logs ?? new())
            {
                l.Id ??= Guid.NewGuid().ToString();
                l.OperationId = dto.Id;
                _db.TransactionLogs.Add(_mapper.Map<TransactionLog>(l));
            }

            _db.SaveChanges();
            tx.Commit();
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
            var entity = _db.Operations.FirstOrDefault(x => x.Id == id)
                         ?? throw new ElementNotFoundException(id);

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
            var entity = _db.Operations.FirstOrDefault(x => x.Id == id)
                         ?? throw new ElementNotFoundException(id);

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

    // --------- helpers for business-logic ---------

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

    /// <summary>
    /// Нужен для валидации: продукция должна принадлежать подразделению.
    /// productId -> departamentId
    /// </summary>
    public Dictionary<string, string?> GetProductionDepartaments(IEnumerable<string> productIds)
    {
        try
        {
            return _db.Productions
                .AsNoTracking()
                .Where(x => productIds.Contains(x.Id))
                .ToDictionary(x => x.Id, x => x.DepartamentId);
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public Dictionary<string, (int qty, decimal sum)> GetReceipts43_20_Plan(DateTime from, DateTime to, string acc43Id, string acc20Id)
        => _db.TransactionLogs.AsNoTracking()
            .Where(t => !t.IsDeleted
                && t.DateOperation >= from && t.DateOperation <= to
                && t.ChartOfAccountDebId == acc43Id
                && t.ChartOfAccountCredId == acc20Id
                && t.Count > 0
                && t.Subconto1Deb != null)
            .GroupBy(t => t.Subconto1Deb!)
            .ToDictionary(g => g.Key, g => (qty: g.Sum(x => x.Count), sum: g.Sum(x => x.Amount)));

    public Dictionary<string, decimal> GetAllocDeltas43_20(DateTime from, DateTime to, string acc43Id, string acc20Id)
        => _db.TransactionLogs.AsNoTracking()
            .Where(t => !t.IsDeleted
                && t.DateOperation >= from && t.DateOperation <= to
                && t.ChartOfAccountDebId == acc43Id
                && t.ChartOfAccountCredId == acc20Id
                && t.Count == 0
                && t.Subconto1Deb != null)
            .GroupBy(t => t.Subconto1Deb!)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

    public Dictionary<string, (int qty, decimal sum)> GetSalesCogs90_43_Plan(DateTime from, DateTime to, string acc90Id, string acc43Id)
        => _db.TransactionLogs.AsNoTracking()
            .Where(t => !t.IsDeleted
                && t.DateOperation >= from && t.DateOperation <= to
                && t.ChartOfAccountDebId == acc90Id
                && t.ChartOfAccountCredId == acc43Id
                && t.Count > 0
                && t.Subconto1Cred != null)
            .GroupBy(t => t.Subconto1Cred!)
            .ToDictionary(g => g.Key, g => (qty: g.Sum(x => x.Count), sum: g.Sum(x => x.Amount)));

    public decimal GetDebitTurnover20(DateTime from, DateTime to, string acc20Id)
        => _db.TransactionLogs.AsNoTracking()
            .Where(t => !t.IsDeleted
                && t.DateOperation >= from && t.DateOperation <= to
                && t.ChartOfAccountDebId == acc20Id)
            .Sum(t => (decimal?)t.Amount) ?? 0m;

    // --------- "историчность на дату операции" ---------

    private void FillNamesByHistoryAt(OperationDto dto)
    {
        if (dto == null) return;

        var atUtc = dto.DateOperation.Kind switch
        {
            DateTimeKind.Utc => dto.DateOperation,
            DateTimeKind.Local => dto.DateOperation.ToUniversalTime(),
            _ => DateTime.SpecifyKind(dto.DateOperation, DateTimeKind.Utc)
        };

        if (!string.IsNullOrWhiteSpace(dto.DepartamentId))
            dto.DepartamentName = ResolveDepartamentNameAt(dto.DepartamentId!, atUtc);

        if (!string.IsNullOrWhiteSpace(dto.OrganisationId))
            dto.OrganisationName = ResolveOrganisationNameAt(dto.OrganisationId!, atUtc);

        if (dto.Elements != null && dto.Elements.Count > 0)
        {
            foreach (var e in dto.Elements)
            {
                if (!string.IsNullOrWhiteSpace(e.ProductionId))
                    e.ProductionName = ResolveProductionNameAt(e.ProductionId!, atUtc);
            }
        }
    }

    private string? ResolveDepartamentNameAt(string departamentId, DateTime atUtc)
    {
        // 1) пробуем историю
        var h = _db.DepartamentHistories
            .AsNoTracking()
            .Where(x => !x.IsDeleted
                && x.DepartamentId == departamentId
                && x.ValidFrom <= atUtc
                && (x.ValidTo == null || atUtc < x.ValidTo))
            .OrderByDescending(x => x.ValidFrom)
            .Select(x => x.Name)
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(h)) return h;

        // 2) fallback на текущее имя
        return _db.Departaments
            .AsNoTracking()
            .Where(x => x.Id == departamentId)
            .Select(x => x.Name)
            .FirstOrDefault();
    }

    private string? ResolveOrganisationNameAt(string organisationId, DateTime atUtc)
    {
        var h = _db.OrganisationHistories
            .AsNoTracking()
            .Where(x => !x.IsDeleted
                && x.OrganisationId == organisationId
                && x.ValidFrom <= atUtc
                && (x.ValidTo == null || atUtc < x.ValidTo))
            .OrderByDescending(x => x.ValidFrom)
            .Select(x => x.Name)
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(h)) return h;

        return _db.Organisations
            .AsNoTracking()
            .Where(x => x.Id == organisationId)
            .Select(x => x.Name)
            .FirstOrDefault();
    }

    private string? ResolveProductionNameAt(string productionId, DateTime atUtc)
    {
        var h = _db.ProductionHistories
            .AsNoTracking()
            .Where(x => !x.IsDeleted
                && x.ProductionId == productionId
                && x.ValidFrom <= atUtc
                && (x.ValidTo == null || atUtc < x.ValidTo))
            .OrderByDescending(x => x.ValidFrom)
            .Select(x => x.Name)
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(h)) return h;

        return _db.Productions
            .AsNoTracking()
            .Where(x => x.Id == productionId)
            .Select(x => x.Name)
            .FirstOrDefault();
    }
}
