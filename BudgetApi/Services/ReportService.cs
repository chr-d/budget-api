using BudgetApi.Dtos.Reports;
using BudgetApi.Models;

namespace BudgetApi.Services;

public class ReportService : IReportService
{
    public Task<SummaryReportResponseDto> GetSummaryAsync(
        DateOnly start,
        DateOnly end,
        TransactionType? type
    )
    {
        throw new NotImplementedException();
    }
}
