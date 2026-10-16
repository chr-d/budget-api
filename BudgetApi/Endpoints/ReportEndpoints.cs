using BudgetApi.Dtos.Reports;
using BudgetApi.Models;
using BudgetApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BudgetApi.Endpoints;

public static class ReportEndpoints
{
    public static RouteGroupBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/reports");
        group.MapGet("/summary", GetSummaryReport);

        return group;
    }

    private static async Task<Ok<SummaryReportResponseDto>> GetSummaryReport(
        DateOnly start,
        DateOnly end,
        TransactionType? type,
        IReportService reportService
    )
    {
        return TypedResults.Ok(await reportService.GetSummaryAsync(start, end, type));
    }
}
