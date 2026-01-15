using AutoMapper;
using Contracts.DTO;
using Contracts.Exceptions;
using Contracts.Interfaces.Storages;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace DataBase.Implementation;

public class OrganisationStorageContract(TwoCDbContext dbContext, IMapper mapper, ILogger<OrganisationStorageContract> logger) : IOrganisationStorageContract
{
    private readonly TwoCDbContext _dbContext = dbContext;
    private IMapper _mapper = mapper;
    public void Create(OrganisationDto organisationDto)
    {
        try
        {
            _dbContext.Organisations.Add(_mapper.Map<Organisation>(organisationDto));
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
            var entity = GetOrganisationById(id);
            entity.IsDeleted = true;
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public List<OrganisationDto> GetAll()
    {
        try
        {
            var result = _dbContext.Organisations.ToList();
            logger.LogInformation("Упал в строаже");
            return [.. result.Select(x => _mapper.Map<OrganisationDto>(x))];
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public OrganisationDto GetById(string id)
    {
        try
        {
            return _mapper.Map<OrganisationDto>(_dbContext
                .Organisations
                .FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    public OrganisationDto GetByName(string name)
    {
        try
        {
            return _mapper.Map<OrganisationDto>(_dbContext
                .Organisations
                .FirstOrDefault(x => x.Name == name));
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
            var element = GetById(id) ?? throw new ElementNotFoundException(id);
            element.IsDeleted = false;
            _dbContext.SaveChanges();
        }
        catch
        {
            _dbContext.ChangeTracker.Clear();
            throw;
        }
    }

    public void Update(OrganisationDto organisationDto)
    {
        var entity = GetOrganisationById(organisationDto.Id);
        try
        {
            _dbContext.Organisations.Update(_mapper.Map(organisationDto, entity));
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            throw new StorageException(ex);
        }
    }

    private Organisation GetOrganisationById(string id) => _dbContext.Organisations.FirstOrDefault(x => x.Id == id);
}
