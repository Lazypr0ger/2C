using AutoMapper;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages;
using DataBase.Entities;

namespace DataBase.Implementation;

public class ChartOfAccountStorageContract : IChartOfAccountStorageContract
{
    private readonly TwoCDbContext _dbContext;
    private readonly IMapper _mapper;
    public ChartOfAccountStorageContract(TwoCDbContext dbContext)
    {
        _dbContext = dbContext;
        var config = new MapperConfiguration();
        _mapper = config.CreateMapper();
    }
    public void Create(ChartOfAccountDto hartOfAccountDto)
    {
        throw new NotImplementedException();
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
