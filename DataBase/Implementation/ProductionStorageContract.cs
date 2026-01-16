using AutoMapper;
using Contracts.DTO;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataBase.Implementation;

public class ProductionStorageContract(TwoCDbContext dbContext, 
    IMapper mapper,ILogger<ProductionStorageContract> logger) : IProductionStorageContract
{
    private readonly TwoCDbContext _dbContext = dbContext;
    private IMapper _mapper = mapper;
    public void Create(ProductionDto productionDto)
    {
        try
        {
            _dbContext.Productions.Add(_mapper.Map<Production>(productionDto));
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
            var entity = GetProductionById(id);
            entity.IsDeleted = true;
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public List<ProductionDto> GetAll()
    {
        try
        {
            var result = _dbContext.Productions.ToList();
            return [.. result.Select(x => _mapper.Map<ProductionDto>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ProductionDto GetByCode(string Code)
    {
        try
        {
            return _mapper.Map<ProductionDto>(_dbContext
            .Productions
            .FirstOrDefault(x => x.Code == Code));
        }
        catch(Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ProductionDto GetById(string Id)
    {
        try
        {
            return _mapper.Map<ProductionDto>(_dbContext
            .Productions
            .FirstOrDefault(x => x.Id == Id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ProductionDto GetByName(string Name)
    {
        try
        {
            return _mapper.Map<ProductionDto>(_dbContext
            .Productions
            .FirstOrDefault(x => x.Name == Name));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public List<ProductionDto> GetByType(TypeProduct typeProduct)
    {
        try
        {
            var products = _dbContext.Productions.Where(x => x.Type == typeProduct).ToList();
            return products.Select(x => _mapper.Map<ProductionDto>(x)).ToList();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void Recovery(string id)
    {
        try
        {
            var products = GetProductionById(id) ?? throw new ElementNotFoundException(id);
            products.IsDeleted = false;
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }

    }

    public void Update(ProductionDto productionDto)
    {
        try
        {
            var products = GetProductionById(productionDto.Id)  ?? throw new ElementNotFoundException(productionDto.Id);
            _dbContext.Productions.Update(_mapper.Map(productionDto, products));
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

    public Production GetProductionById(string id) => _dbContext.Productions.FirstOrDefault(x => x.Id == id);

    public List<ProductionDto> GetProductByDepartamentName(string departamentName)
    {
        try 
        {
           var departamentId = _dbContext.Departaments.Where(x => x.Name == departamentName).FirstOrDefault().Id;
            return [.. _dbContext.Productions.Where(x => x.DepartamentId == departamentId).Select(y => _mapper.Map<ProductionDto>(y))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }
}
