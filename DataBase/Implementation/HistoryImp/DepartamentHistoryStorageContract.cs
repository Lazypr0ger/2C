using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.DTO.HistoriesDto;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages.HistoryStorageContracts;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Implementation.HistoryImp
{
    public class DepartamentHistoryStorageContract : IDepartamentHistoryStorageContract
    {
        private readonly TwoCDbContext _db;
        private readonly IMapper _mapper;

        public DepartamentHistoryStorageContract(TwoCDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public List<DepartamentHistoryDto> GetByDepartamentId(string departamentId)
        {
            try
            {
                var list = _db.DepartamentHistories
                    .AsNoTracking()
                    .Where(x => x.DepartamentId == departamentId)
                    .OrderByDescending(x => x.ValidFrom)
                    .ToList();

                return list.Select(x => _mapper.Map<DepartamentHistoryDto>(x)).ToList();
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                throw new StorageException(ex);
            }
        }

        public DepartamentDto GetAsOf(string departamentId, DateTime atUtc)
        {
            try
            {
                var snap = _db.DepartamentHistories
                    .AsNoTracking()
                    .Where(x => x.DepartamentId == departamentId
                                && x.ValidFrom <= atUtc
                                && (x.ValidTo == null || x.ValidTo > atUtc))
                    .OrderByDescending(x => x.ValidFrom)
                    .FirstOrDefault();

                if (snap != null)
                {
                    return new DepartamentDto
                    {
                        Id = departamentId,
                        Name = snap.Name,
                        IsDeleted = snap.IsDeleted
                    };
                }

                var current = _db.Departaments
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == departamentId);

                if (current == null)
                    throw new ElementNotFoundException(departamentId);

                return _mapper.Map<DepartamentDto>(current);
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
                var history = _db.DepartamentHistories
                    .FirstOrDefault(x => x.Id == historyId)
                    ?? throw new ElementNotFoundException(historyId);

                var dep = _db.Departaments
                    .FirstOrDefault(x => x.Id == history.DepartamentId)
                    ?? throw new ElementNotFoundException(history.DepartamentId);

                dep.Name = history.Name ?? dep.Name;
                dep.IsDeleted = history.IsDeleted;

                _db.SaveChanges();
            }
            catch
            {
                _db.ChangeTracker.Clear();
                throw;
            }
        }
    }
}
