using System.Reflection;
using Carter;
using JasperFx;
using Wolverine;
using Wolverine.Http;
using Yestino.Ordering;
using Yestino.ProductCatalog;
using Yestino.Warehouse;
using Yestino.Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.SetupWolverine();

builder.AddProductCatalogModule();
builder.AddWarehouseModule();
// TODO: register modules here

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.Services.ApplyAsyncWolverineExtensions();
app.MapWolverineEndpoints();

app.MapCarter();

app.UseHttpsRedirection();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

return await app.RunJasperFxCommands(args.Length == 0 ? ["run"] : args);