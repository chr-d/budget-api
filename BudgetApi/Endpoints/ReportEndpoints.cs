using BudgetApi.Dtos.Reports;
using BudgetApi.Models;
using BudgetApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BudgetApi.Endpoints;

public static class ReportEndpoints
{
    // TODO: remove this when proper authentication and authorization is in place
    private static readonly Guid _devUser = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static RouteGroupBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/reports").WithTags("Reports");
        group
            .MapGet("/summary", GetSummaryReport)
            .WithName("GetSummaryReport")
            .WithSummary("Get a summary report")
            .WithDescription(
                "Returns a summary report for the date range with optional type filter."
            );

        return group;
    }

    private static async Task<
        Results<Ok<SummaryReportResponseDto>, ValidationProblem>
    > GetSummaryReport(
        DateOnly start,
        DateOnly end,
        TransactionType? type,
        IReportService reportService
    )
    {
        if (start > end)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["end"] = ["The end date must be greater than or equal to the start date."],
                }
            );
        }

        return TypedResults.Ok(await reportService.GetSummaryAsync(_devUser, start, end, type));
    }
}
