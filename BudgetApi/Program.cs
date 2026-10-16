using System.Text.Json.Serialization;
using BudgetApi.Endpoints;
using BudgetApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter())
);

builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<IReportService, ReportService>();

var app = builder.Build();

var api = app.MapGroup("/api");
api.MapTransactionEndpoints();
api.MapReportEndpoints();

app.Run();
