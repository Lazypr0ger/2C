using Contracts;
using Contracts.AdapterContracts;
using Contracts.Interfaces.Storages;
using DataBase;
using DataBase.Implementation;
using MiddleServer;
using MiddleServer.Adapters;
using Serilog;
using BusinessLogic;
using Contracts.Interfaces.Business;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

using var loggerFactory = new LoggerFactory();
loggerFactory.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger());
builder.Services.AddSingleton(loggerFactory.CreateLogger("Any"));
builder.Services.AddOpenApi();

builder.Services.AddTransient<TwoCDbContext>();

builder.Services.AddSingleton<IConfigurationDatabase, ConfigurationDatabase>();

builder.Services.AddTransient<IChartOfAccountStorageContract, ChartOfAccountStorageContract>();
builder.Services.AddTransient<IChartOfAccountBusinessLogic, ChartOfAccountBusinessLogic>();

builder.Services.AddTransient<IChartOfAccountAdapterContract, ChartOfAccountAdapter>();

builder.Services.AddSwaggerGen();

var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
