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
    public List<TransactionLogDto> GetView(DateTime? from = null, DateTime? to = null)
    {
        try
        {
            var q = _db.TransactionLogs
                .AsNoTracking()
                .Where(t => !t.IsDeleted);

            if (from.HasValue) q = q.Where(x => x.DateOperation >= from.Value);
            if (to.HasValue) q = q.Where(x => x.DateOperation <= to.Value);

            var query =
                from t in q
                join deb in _db.ChartOfAccounts.AsNoTracking() on t.ChartOfAccountDebId equals deb.Id
                join cred in _db.ChartOfAccounts.AsNoTracking() on t.ChartOfAccountCredId equals cred.Id

                // Organisation (покупатель) история — используем, когда Subconto содержит OrganisationId
                let orgDebName =
                    (from h in _db.OrganisationHistories.AsNoTracking()
                     where h.OrganisationId == t.Subconto1Deb
                           && h.ValidFrom <= t.DateOperation
                           && (h.ValidTo == null || t.DateOperation < h.ValidTo)
                     orderby h.ValidFrom descending
                     select h.Name).FirstOrDefault()

                let orgCredName =
                    (from h in _db.OrganisationHistories.AsNoTracking()
                     where h.OrganisationId == t.Subconto1Cred
                           && h.ValidFrom <= t.DateOperation
                           && (h.ValidTo == null || t.DateOperation < h.ValidTo)
                     orderby h.ValidFrom descending
                     select h.Name).FirstOrDefault()

                // Departament история
                let depDebName =
                    (from h in _db.DepartamentHistories.AsNoTracking()
                     where h.DepartamentId == t.Subconto1Deb
                           && h.ValidFrom <= t.DateOperation
                           && (h.ValidTo == null || t.DateOperation < h.ValidTo)
                     orderby h.ValidFrom descending
                     select h.Name).FirstOrDefault()

                let depCredName =
                    (from h in _db.DepartamentHistories.AsNoTracking()
                     where h.DepartamentId == t.Subconto1Cred
                           && h.ValidFrom <= t.DateOperation
                           && (h.ValidTo == null || t.DateOperation < h.ValidTo)
                     orderby h.ValidFrom descending
                     select h.Name).FirstOrDefault()

                // Production история
                let prodDebName =
                    (from h in _db.ProductionHistories.AsNoTracking()
                     where h.ProductionId == t.Subconto1Deb
                           && h.ValidFrom <= t.DateOperation
                           && (h.ValidTo == null || t.DateOperation < h.ValidTo)
                     orderby h.ValidFrom descending
                     select h.Name).FirstOrDefault()

                let prodCredName =
                    (from h in _db.ProductionHistories.AsNoTracking()
                     where h.ProductionId == t.Subconto1Cred
                           && h.ValidFrom <= t.DateOperation
                           && (h.ValidTo == null || t.DateOperation < h.ValidTo)
                     orderby h.ValidFrom descending
                     select h.Name).FirstOrDefault()

                select new TransactionLogDto
                {
                    Id = t.Id,
                    DateOperation = t.DateOperation,

                    Subconto1Deb = t.Subconto1Deb,
                    Subconto2Deb = t.Subconto2Deb,
                    Subconto1Cred = t.Subconto1Cred,
                    Subconto2Cred = t.Subconto2Cred,

                    Amount = t.Amount,
                    Count = t.Count,
                    Comment = t.Comment,
                    OperationId = t.OperationId,

                    ChartOfAccountDebId = t.ChartOfAccountDebId,
                    ChartOfAccountCredId = t.ChartOfAccountCredId,

                    IsDeleted = t.IsDeleted,

                    ChartDebNum = deb.NumChart,
                    ChartDebName = deb.Name,
                    ChartCredNum = cred.NumChart,
                    ChartCredName = cred.Name,

                    Subconto1DebName = orgDebName ?? depDebName ?? prodDebName,
                    Subconto1CredName = orgCredName ?? depCredName ?? prodCredName,
                };

            return query
                .OrderBy(x => x.DateOperation)
                .ToList();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }
    private TransactionLog? GetEntity(string? id)
        => string.IsNullOrWhiteSpace(id) ? null : _db.TransactionLogs.FirstOrDefault(x => x.Id == id);
}
