using AutoMapper;
using Contracts;
using Contracts.AdapterContracts;
using Contracts.Interfaces.Storages;
using DataBase;
using DataBase.Implementation;
using MiddleServer;
using MiddleServer.Adapters;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TwoCDbContext>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();

builder.Services.AddSingleton<IConfigurationDatabase, ConfigurationDatabase>();


builder.Services.AddTransient<TwoCDbContext>();
builder.Services.AddTransient<IChartOfAccountStorageContract, ChartOfAccountStorageContract>();


builder.Services.AddTransient<IChartOfAccountAdapterContract, ChartOfAccountAdapter>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
