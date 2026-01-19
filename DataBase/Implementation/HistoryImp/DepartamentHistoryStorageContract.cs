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
                // Ищем snapshot-версию на дату
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

                // Если не было ни одной записи истории — значит сущность никогда не менялась
                // Берем текущее состояние
                var current = _db.Departaments
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == departamentId);

                if (current == null) throw new ElementNotFoundException(departamentId);

                return _mapper.Map<DepartamentDto>(current);
            }
            catch
            {
                _db.ChangeTracker.Clear();
                throw;
            }
        }
    }
}
