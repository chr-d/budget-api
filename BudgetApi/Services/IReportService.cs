using BudgetApi.Dtos.Reports;
using BudgetApi.Models;

namespace BudgetApi.Services;

public interface IReportService
{
    Task<SummaryReportResponseDto> GetSummaryAsync(
        Guid userId,
        DateOnly start,
        DateOnly end,
        TransactionType? type
    );
}
