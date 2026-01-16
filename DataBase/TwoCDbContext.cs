using Contracts;
using DataBase.Entities;
using DataBase.Entities.HistoriesModel;
using Microsoft.EntityFrameworkCore;
using Contracts.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase;

public class TwoCDbContext : DbContext
{
    public TwoCDbContext(DbContextOptions<TwoCDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<ChartOfAccount>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.NumChart).IsUnique();

            entity.Property(x => x.NumChart)
                  .HasMaxLength(20)
                  .IsRequired();

            entity.Property(x => x.Name)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(x => x.Subconto1)
                  .HasMaxLength(100);

            entity.Property(x => x.Subconto2)
                  .HasMaxLength(100);
        });


        modelBuilder.Entity<Departament>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique();

            entity.Property(x => x.Name)
                  .HasMaxLength(100)
                  .IsRequired();
        });


        modelBuilder.Entity<Production>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => new { x.Code, x.Name }).IsUnique();

            entity.Property(x => x.Code)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(x => x.Name)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(x => x.PlannedCost)
                  .HasPrecision(18, 2);

            entity.Property(x => x.Type)
                  .HasConversion<string>()
                  .HasMaxLength(50);

            // Связь с Departament
            entity.HasOne(x => x.Departament)
                  .WithMany(x => x.Productions)
                  .HasForeignKey(x => x.DepartamentId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Связь с ProductionHistories
            entity.HasMany(x => x.ProductionHistories)
                  .WithOne(x => x.Production)
                  .HasForeignKey(x => x.ProductionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Organisation>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.AccountNumOrg).IsUnique();

            entity.Property(x => x.Name)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(x => x.AccountNumOrg)
                  .HasMaxLength(20)
                  .IsRequired();

            entity.HasMany(x => x.OrganisationHistories)
                  .WithOne(x => x.Organisation)
                  .HasForeignKey(x => x.OrganisationId);
        });


        modelBuilder.Entity<Element>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.CostRealisation)
                  .HasPrecision(18, 4);

            entity.HasOne(x => x.Production)
                  .WithMany(x => x.Elements)
                  .HasForeignKey(x => x.ProductionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Operation)
                  .WithMany(x => x.Element)
                  .HasForeignKey(x => x.OperationId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.NameDocument)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(x => x.Agent)
                  .HasMaxLength(150)
                  .IsRequired();

            entity.Property(x => x.TotalAmountDocument)
                  .HasPrecision(18, 2);

            entity.Property(x => x.Type)
                  .HasConversion<string>()
                  .HasMaxLength(50);

            entity.HasOne(x => x.Organisation)
                  .WithMany(x => x.Operation)
                  .HasForeignKey(x => x.OrganisationId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.DateOperation);
            entity.HasIndex(x => new { x.Type, x.DateOperation });
        });


        modelBuilder.Entity<TransactionLog>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Subconto1Deb)
                  .HasPrecision(18, 4);

            entity.Property(x => x.Subconto2Deb)
                  .HasPrecision(18, 4);

            entity.Property(x => x.Subconto1Cred)
                  .HasPrecision(18, 4);

            entity.Property(x => x.Subconto2Cred)
                  .HasPrecision(18, 4);

            entity.Property(x => x.Amount)
                  .HasPrecision(18, 4);

            entity.Property(x => x.Comment)
                  .HasMaxLength(500);

            entity.HasOne(x => x.Operation)
                  .WithMany(x => x.TransactionLog)
                  .HasForeignKey(x => x.OperationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ChartOfAccountDeb)
                  .WithMany(x => x.TransactionLogDeb)
                  .HasForeignKey(x => x.ChartOfAccountDebId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ChartOfAccountCred)
                  .WithMany(x => x.TransactionLogCred)
                  .HasForeignKey(x => x.ChartOfAccountCredId)
                  .OnDelete(DeleteBehavior.Restrict);


            entity.HasIndex(x => x.DateOperation);
            entity.HasIndex(x => new { x.ChartOfAccountDebId, x.DateOperation });
            entity.HasIndex(x => new { x.ChartOfAccountCredId, x.DateOperation });
        });

        modelBuilder.Entity<DepartamentHistory>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.OldName)
                  .HasMaxLength(100);


            entity.HasOne(x => x.Departament)
                  .WithMany(x => x.DepartamentHistories)
                  .HasForeignKey(x => x.DepartamentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.DepartamentId);
            entity.HasIndex(x => x.ValidFrom);
            entity.HasIndex(x => x.ValidTo);
        });

        modelBuilder.Entity<OrganisationHistory>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.OldName)
                  .HasMaxLength(200);

            entity.Property(x => x.OldAccountNumOrg)
                  .HasMaxLength(20);

            entity.HasOne(x => x.Organisation)
                  .WithMany(x => x.OrganisationHistories)
                  .HasForeignKey(x => x.OrganisationId)
                  .OnDelete(DeleteBehavior.Cascade);

 
            entity.HasIndex(x => x.OrganisationId);
            entity.HasIndex(x => x.ValidFrom);
        });


        modelBuilder.Entity<ProductionHistory>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.OldCode)
                  .HasMaxLength(50);

            entity.Property(x => x.OldName)
                  .HasMaxLength(200);

            entity.Property(x => x.OldPlannedCost)
                  .HasPrecision(18, 2);

            entity.Property(x => x.OldType)
                  .HasConversion<string>()
                  .HasMaxLength(50);

            // Связь с Production
            entity.HasOne(x => x.Production)
                  .WithMany(x => x.ProductionHistories)
                  .HasForeignKey(x => x.ProductionId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            entity.HasIndex(x => x.ProductionId);
            entity.HasIndex(x => x.ValidFrom);
            entity.HasIndex(x => x.ValidTo);
        });
    }

    public DbSet<ChartOfAccount> ChartOfAccounts { get; set; }
    public DbSet<Departament> Departaments { get; set; }
    public DbSet<Production> Productions { get; set; }
    public DbSet<Organisation> Organisations { get; set; }
    public DbSet<Element> Elements { get; set; }
    public DbSet<Operation> Operations { get; set; }
    public DbSet<TransactionLog> TransactionLogs { get; set; }
    public DbSet<DepartamentHistory> DepartamentHistories { get; set; }
    public DbSet<OrganisationHistory> OrganisationHistories { get; set; }
    public DbSet<ProductionHistory> ProductionHistories { get; set; }
}