using Contracts;
using Contracts.AdapterContracts;
using Contracts.Interfaces.Storages;
using DataBase;
using DataBase.Implementation;
using Serilog;
using BusinessLogic;
using Contracts.Interfaces.Business;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MainServer;
using MainServer.Adapters;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

using var loggerFactory = new LoggerFactory();
loggerFactory.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger());
builder.Services.AddSingleton(loggerFactory.CreateLogger("Any"));
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TwoCDbContext>((sp, options) =>
{
    var cfg = sp.GetRequiredService<IConfigurationDatabase>();
    options.UseNpgsql(cfg.ConnectionString, o => o.SetPostgresVersion(16, 2));
});
try
{
    builder.Services.AddAutoMapper(cfg =>
    {
        cfg.AddProfile<DtoToVmProfile>();
        cfg.AddProfile<DtoToEntityProfile>();
        cfg.AddProfile<BindModelToDtoProfile>();
    });
}
catch (ReflectionTypeLoadException ex)
{
    var lines = ex.LoaderExceptions.Select(e => e?.ToString() ?? "<null>").ToArray();
    File.WriteAllLines("typeload.txt", lines);
    throw;
}

builder.Services.AddTransient<TwoCDbContext>();

builder.Services.AddSingleton<IConfigurationDatabase, ConfigurationDatabase>();

//business registrate
builder.Services.AddTransient<IChartOfAccountBusinessLogic, ChartOfAccountBusinessLogic>();
builder.Services.AddTransient<IDepartamentBusinessLogic, DepartamentBusinessLogic>();
builder.Services.AddTransient<IOrganisationBusinessLogic, OrganisationBusinessLogic>();
builder.Services.AddTransient<IProductionBusinessLogic, ProductionBusinessLogic>();
builder.Services.AddTransient<IElementBusinessLogic, ElementBusinessLogic>();
builder.Services.AddTransient<IOperationBusinessLogic, OperationBusinessLogic>();
builder.Services.AddTransient<ITransactionLogBusinessLogic, TransactionLogBusinessLogic>();


//storages registrate
builder.Services.AddTransient<IChartOfAccountStorageContract, ChartOfAccountStorageContract>();
builder.Services.AddTransient<IDepartamentStorageContract, DepartamentStorageContract>();
builder.Services.AddTransient<IOrganisationStorageContract, OrganisationStorageContract>();
builder.Services.AddTransient<IProductionStorageContract, ProductionStorageContract>();
builder.Services.AddTransient<IElementStorageContract, ElementStorageContract>();
builder.Services.AddTransient<IOperationStorageContract, OperationStorageContract>();
builder.Services.AddTransient<ITransactionLogStorageContract, TransactionLogStorageContract>();


// Adapters registrate
builder.Services.AddTransient<IChartOfAccountAdapterContract, ChartOfAccountAdapter>();
builder.Services.AddTransient<IDepartamentAdapterContract, DepartamentAdapter>();
builder.Services.AddTransient<IOrganisationAdapterContract, OrganisationAdapter>();
builder.Services.AddTransient<IProductionAdapterContract, ProductionAdapter>();
builder.Services.AddTransient<IElementAdapterContract, ElementAdapter>();
builder.Services.AddTransient<IOperationAdapterContract, OperationAdapter>();
builder.Services.AddTransient<ITransactionLogAdapterContract, TransactionLogAdapter>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //using var scope = app.Services.CreateScope();
    //var context = scope.ServiceProvider.GetRequiredService<TwoCDbContext>;

    //context.Database.Migarte();

    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
