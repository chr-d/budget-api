using System.Text.Json.Serialization;
using BudgetApi.Endpoints;
using BudgetApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter())
);

builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<IReportService, ReportService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var api = app.MapGroup("/api");
api.MapTransactionEndpoints();
api.MapReportEndpoints();

app.Run();
