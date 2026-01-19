using AutoMapper;
using Contracts.DTO;
using Contracts.DTO.HistoriesDto;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages.HistoryStorageContracts;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Implementation.HistoryImp;

public class ProductionHistoryStorageContract : IProductionHistoryStorageContract
{
    private readonly TwoCDbContext _db;
    private readonly IMapper _mapper;

    public ProductionHistoryStorageContract(TwoCDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public List<ProductionHistoryDto> GetByProductionId(string productionId)
    {
        try
        {
            var list = _db.ProductionHistories
                .AsNoTracking()
                .Where(x => x.ProductionId == productionId)
                .OrderByDescending(x => x.ValidFrom)
                .ToList();

            return list.Select(x => _mapper.Map<ProductionHistoryDto>(x)).ToList();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ProductionDto GetAsOf(string productionId, DateTime atUtc)
    {
        try
        {
            var snap = _db.ProductionHistories
                .AsNoTracking()
                .Where(x => x.ProductionId == productionId
                            && x.ValidFrom <= atUtc
                            && (x.ValidTo == null || x.ValidTo > atUtc))
                .OrderByDescending(x => x.ValidFrom)
                .FirstOrDefault();

            if (snap != null)
            {
                return new ProductionDto
                {
                    Id = productionId,
                    Code = snap.Code,
                    Name = snap.Name,
                    PlannedCost = snap.PlannedCost,
                    Type = snap.Type,
                    DepartamentId = snap.DepartamentId,
                    IsDeleted = snap.IsDeleted
                };
            }

            var current = _db.Productions
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == productionId);

            if (current == null)
                throw new ElementNotFoundException(productionId);

            return _mapper.Map<ProductionDto>(current);
        }
        catch
        {
            _db.ChangeTracker.Clear();
            throw;
        }
    }

    public void RestoreFromHistory(string historyId)
    {
        try
        {
            var history = _db.ProductionHistories
                .FirstOrDefault(x => x.Id == historyId)
                ?? throw new ElementNotFoundException(historyId);

            var prod = _db.Productions
                .FirstOrDefault(x => x.Id == history.ProductionId)
                ?? throw new ElementNotFoundException(history.ProductionId);

            prod.Code = history.Code;
            prod.Name = history.Name;
            prod.PlannedCost = history.PlannedCost;
            prod.Type = history.Type;
            prod.DepartamentId = history.DepartamentId;
            prod.IsDeleted = history.IsDeleted;

            _db.SaveChanges();
        }
        catch
        {
            _db.ChangeTracker.Clear();
            throw;
        }
    }
}
