using Contracts;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace DataBase;

public class TwoCDbContext : DbContext
{
    private readonly IConfigurationDatabase _configDatabase;
    public TwoCDbContext(IConfigurationDatabase configurationDatabase) 
    { 
        _configDatabase = configurationDatabase;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configDatabase == null || string.IsNullOrEmpty(_configDatabase.ConnectionString))
            throw new InvalidOperationException("Database configuration is not set");

        optionsBuilder.UseNpgsql(_configDatabase.ConnectionString, o => o.SetPostgresVersion(16, 2));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ChartOfAccount>().HasIndex(x => x.NumChart).IsUnique();
        modelBuilder.Entity<ChartOfAccount>()
            .HasMany(x => x.Departament)
            .WithOne(y => y.ChartOfAccount);
    }

    public DbSet<ChartOfAccount> ChartOfAccount { get; set; }
    public DbSet<Departament> Departament {  get; set; }
    public DbSet<Production> Production { get; set; }  
    public DbSet<Organisation> Organisation { get; set; }
    public DbSet<Element> Element { get; set; }

    public DbSet<Operation> Operation { get; set; }
    public DbSet<TransactionLog> TransactionLog { get; set; }

}
