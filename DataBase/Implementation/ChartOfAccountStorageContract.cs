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
            _dbContext.ChartOfAccount.Add(_mapper.Map<ChartOfAccount>(chartOfAccountDto));
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
        throw new NotImplementedException();
    }

    public List<ChartOfAccountDto> GetAll()
    {
        try
        {
            var query = _dbContext.ChartOfAccount.AsQueryable();

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

    public ChartOfAccountDto GetById(int id)
    {
        throw new NotImplementedException();
    }

    public ChartOfAccountDto GetByNameChart(string NameChart)
    {
        throw new NotImplementedException();
    }

    public ChartOfAccountDto GetByNumChart(string NumChart)
    {
        throw new NotImplementedException();
    }

    public void Update(ChartOfAccountDto hartOfAccountDto)
    {
        throw new NotImplementedException();
    }
}
