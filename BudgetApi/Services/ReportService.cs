using BudgetApi.Data;
using BudgetApi.Dtos.Reports;
using BudgetApi.Dtos.Transactions;
using BudgetApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetApi.Services;

public class ReportService(ApplicationDbContext db) : IReportService
{
    private readonly ApplicationDbContext _db = db;

    public async Task<SummaryReportResponseDto> GetSummaryAsync(
        Guid userId,
        DateOnly start,
        DateOnly end,
        TransactionType? type
    )
    {
        var from = new DateTimeOffset(start.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

        var to = new DateTimeOffset(end.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

        var query = _db
            .Transactions.AsNoTracking()
            .Where(t => t.UserId == userId && t.Timestamp >= from && t.Timestamp < to);

        if (type is not null)
        {
            query = query.Where(t => t.Type == type);
        }

        var totals = await query
            .GroupBy(t => t.Type)
            .Select(g => new { Type = g.Key, Total = g.Sum(t => t.Amount) })
            .ToListAsync();

        var totalIncome = totals.FirstOrDefault(t => t.Type == TransactionType.Income)?.Total ?? 0m;

        var totalExpense =
            totals.FirstOrDefault(t => t.Type == TransactionType.Expense)?.Total ?? 0m;

        IReadOnlyList<TransactionResponseDto> transactions = await query
            .OrderByDescending(t => t.Timestamp)
            .ThenByDescending(t => t.Id)
            .Select(t => new TransactionResponseDto(
                t.Id,
                t.Timestamp,
                t.Type,
                t.Description,
                t.Amount
            ))
            .ToListAsync();

        return new SummaryReportResponseDto(
            start,
            end,
            type,
            totalIncome,
            totalExpense,
            totalIncome - totalExpense,
            transactions
        );
    }
}
