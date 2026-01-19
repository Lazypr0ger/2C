using AutoMapper;
using Contracts.DTO;
using Contracts.DTO.HistoriesDto;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages.HistoryStorageContracts;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Implementation.HistoryImp;

public class OrganisationHistoryStorageContract : IOrganisationHistoryStorageContract
{
    private readonly TwoCDbContext _db;
    private readonly IMapper _mapper;

    public OrganisationHistoryStorageContract(TwoCDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public List<OrganisationHistoryDto> GetByOrganisationId(string organisationId)
    {
        try
        {
            var list = _db.OrganisationHistories
                .AsNoTracking()
                .Where(x => x.OrganisationId == organisationId)
                .OrderByDescending(x => x.ValidFrom)
                .ToList();

            return list.Select(x => _mapper.Map<OrganisationHistoryDto>(x)).ToList();
        }
        catch (Exception ex)
        {
            _db.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public OrganisationDto GetAsOf(string organisationId, DateTime atUtc)
    {
        try
        {
            var snap = _db.OrganisationHistories
                .AsNoTracking()
                .Where(x => x.OrganisationId == organisationId
                            && x.ValidFrom <= atUtc
                            && (x.ValidTo == null || x.ValidTo > atUtc))
                .OrderByDescending(x => x.ValidFrom)
                .FirstOrDefault();

            if (snap != null)
            {
                return new OrganisationDto
                {
                    Id = organisationId,
                    Name = snap.Name,
                    AccountNumOrg = snap.AccountNumOrg,
                    IsDeleted = snap.IsDeleted
                };
            }

            var current = _db.Organisations
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == organisationId);

            if (current == null)
                throw new ElementNotFoundException(organisationId);

            return _mapper.Map<OrganisationDto>(current);
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
            var history = _db.OrganisationHistories
                .FirstOrDefault(x => x.Id == historyId)
                ?? throw new ElementNotFoundException(historyId);

            var org = _db.Organisations
                .FirstOrDefault(x => x.Id == history.OrganisationId)
                ?? throw new ElementNotFoundException(history.OrganisationId);

            org.Name = history.Name;
            org.AccountNumOrg = history.AccountNumOrg;
            org.IsDeleted = history.IsDeleted;

            _db.SaveChanges();
        }
        catch
        {
            _db.ChangeTracker.Clear();
            throw;
        }
    }
}
