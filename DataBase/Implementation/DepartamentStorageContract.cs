using AutoMapper;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DataBase.Implementation;

public class DepartamentStorageContract : IDepartamentStorageContract
{
    private readonly TwoCDbContext _dbContext;
    private readonly DepartamentDto _departement;
    private readonly ChartOfAccountDto _chartOfAccountDto;
    private IMapper _mapper;

    public DepartamentStorageContract(TwoCDbContext dbContext, DepartamentDto departement, ChartOfAccountDto chartOfAccountDto, IMapper mapper)
    {
        _dbContext = dbContext;
        _departement = departement;
        _chartOfAccountDto = chartOfAccountDto;
        _mapper = mapper;
    }

    public void Create(DepartamentDto departamentsDto)
    {
        try
        {
            _dbContext.Departament.Add(_mapper.Map<Departament>(departamentsDto));
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void Delete(string id)
    {
        try
        {
            var entity = GetDepartamentById(id);
            entity.IsDeleted = true;
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
       
    }

    public List<DepartamentDto> GetAll()
    {
        try
        {
            var query = _dbContext.ChartOfAccount.AsQueryable();

            return [.. query
                .Select(x => _mapper
                .Map<DepartamentDto>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public DepartamentDto GetById(string id)
    {
        try
        {
            return _mapper.Map<DepartamentDto>(_dbContext
                .Departament
                .FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public DepartamentDto GetByName(string name)
    {
        return _mapper.Map<DepartamentDto>(_dbContext
               .Departament
               .FirstOrDefault(x => x.Name == name));
    }

    public void Update(DepartamentDto departamentsDto)
    {
        try
        {
            var element = GetDepartamentById(departamentsDto.Id) ?? throw new ElementNotFoundException(departamentsDto.Id);
            _dbContext.Departament.Update(_mapper.Map(departamentsDto, element));
            _dbContext.SaveChanges();
        }
        catch (ElementNotFoundException ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }

    }

    private Departament GetDepartamentById(string id)=>_dbContext.Departament.FirstOrDefault(x => x.Id == id);
}
