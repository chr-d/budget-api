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
        DateOnly start,
        DateOnly end,
        TransactionType? type
    )
    {
        var from = new DateTimeOffset(start.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

        var to = new DateTimeOffset(end.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

        var query = _db
            .Transactions.AsNoTracking()
            .Where(t => t.Timestamp >= from && t.Timestamp < to);

        if (type is not null)
        {
            query = query.Where(t => t.Type == type);
        }

        var totalIncome = await query
            .Where(t => t.Type == TransactionType.Income)
            .SumAsync(t => t.Amount);

        var totalExpense = await query
            .Where(t => t.Type == TransactionType.Expense)
            .SumAsync(t => t.Amount);

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
