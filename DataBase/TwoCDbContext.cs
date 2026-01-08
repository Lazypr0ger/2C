using Contracts;
using DataBase.Entities;
using Microsoft.EntityFrameworkCore;
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

        modelBuilder.Entity<Departament>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Departament>()
            .HasOne(x => x.ChartOfAccount)
            .WithMany(x => x.Departament)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Production>().HasIndex(x => new { x.Code, x.Name }).IsUnique();
        modelBuilder.Entity<Production>()
            .HasOne(x => x.Departament)
            .WithMany(x => x.Production)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Production>()
            .HasOne(x => x.ChartOfAccount)
            .WithMany(x => x.Production);

        modelBuilder.Entity<Element>()
            .HasOne(x => x.Production)
            .WithMany(x => x.Elements)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Element>()
            .HasOne(x => x.Operation)
            .WithMany(x => x.Element)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Operation>()
            .HasOne(x => x.Organisation)
            .WithMany(x => x.Operation);

        modelBuilder.Entity<TransactionLog>()
            .HasOne(x => x.Operation)
            .WithMany(x => x.TransactionLog);

        modelBuilder.Entity<TransactionLog>()
            .HasOne(x => x.ChartOfAccount)
            .WithMany(x => x.TransactionLog1)
            .HasForeignKey(x => x.ChartOfAccountId);

        modelBuilder.Entity<TransactionLog>()
            .HasOne(x => x.ChartOfAccount2)
            .WithMany(x => x.TransactionLog2)
            .HasForeignKey(x => x.ChartOfAccount2Id);

    }

    public DbSet<ChartOfAccount> ChartOfAccount { get; set; }
    public DbSet<Departament> Departament {  get; set; }
    public DbSet<Production> Production { get; set; }  
    public DbSet<Organisation> Organisation { get; set; }
    public DbSet<Element> Element { get; set; }

    public DbSet<Operation> Operation { get; set; }
    public DbSet<TransactionLog> TransactionLog { get; set; }

}
