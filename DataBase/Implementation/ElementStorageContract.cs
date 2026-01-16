using AutoMapper;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages;
using DataBase.Entities;
using Microsoft.Extensions.Logging;

namespace DataBase.Implementation;

public class ElementStorageContract(TwoCDbContext dbContext,
    IMapper mapper,ILogger<ElementStorageContract> logger) : IElementStorageContract
{
    private readonly ILogger<ElementStorageContract> _logger = logger;
    private readonly TwoCDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    public void Create(ElementDto elementDto)
    {
        try
        {
            _dbContext.Elements.Add(_mapper.Map<Element>(elementDto));
            _dbContext.SaveChanges();
        }
        catch(Exception ex) 
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public void Delete(string id)
    {
        try
        {
            var element = GetElementById(id);
            element.IsDeleted = true;
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public List<ElementDto> GetAll()
    {
        try
        {
            return [.. _dbContext.Elements.Select(x => _mapper.Map<ElementDto>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
       
    }

    public List<ElementDto> GetAllByOperation(int id)
    {
        throw new NotImplementedException();
    }

    public ElementDto GetById(string id)
    {
        try
        {
            return  _mapper.Map<ElementDto>(_dbContext.Elements.FirstOrDefault(x => x.Id == id));
        }
        catch(Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public ElementDto GetByOrder(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(ElementDto elementDto)
    {
        try
        {
            var element = GetElementById(elementDto.Id);
            _dbContext.Elements.Update(_mapper.Map(elementDto, element));
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }
    private Element GetElementById(string id) => _dbContext.Elements.FirstOrDefault(x => x.Id == id);

}
