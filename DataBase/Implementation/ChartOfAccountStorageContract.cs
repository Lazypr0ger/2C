using AutoMapper;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace DataBase.Implementation;

public class ChartOfAccountStorageContract : IChartOfAccountStorageContract
{
    private readonly TwoCDbContext _dbContext;
    private readonly IMapper _mapper;
    public ChartOfAccountStorageContract(TwoCDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public void Create(ChartOfAccountDto chartOfAccountDto)
    {
        try
        {
            _dbContext.ChartOfAccounts.Add(_mapper.Map<ChartOfAccount>(chartOfAccountDto));
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public List<ChartOfAccountDto> GetAll()
    {
        try
        {
            var query = _dbContext.ChartOfAccounts.AsQueryable();

            return [.. query
                .Select(x => _mapper
                .Map<ChartOfAccountDto>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ChartOfAccountDto GetById(string id)
    {
        try
        {
            return _mapper
                .Map<ChartOfAccountDto>(_dbContext
                .ChartOfAccounts
                .FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ChartOfAccountDto GetByNameChart(string Name)
    {
        try
        {
            return _mapper
                .Map<ChartOfAccountDto>(_dbContext
                .ChartOfAccounts
                .FirstOrDefault(x => x.Name == Name));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ChartOfAccountDto GetByNumChart(string NumChart)
    {
        try
        {
            return _mapper
                .Map<ChartOfAccountDto>(_dbContext
                .ChartOfAccounts
                .FirstOrDefault(x => x.NumChart == NumChart));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }
}
