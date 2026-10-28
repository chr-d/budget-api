using BudgetApi.Data;
using BudgetApi.Dtos.Transactions;
using BudgetApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetApi.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _db;

    public TransactionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<TransactionResponseDto> CreateAsync(CreateTransactionDto dto)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Timestamp = dto.Timestamp ?? DateTimeOffset.UtcNow,
            Type = dto.Type!.Value,
            Description = dto.Description,
            Amount = dto.Amount,
        };

        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();
        return await Task.FromResult(ToDto(transaction));
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var transaction = await _db.Transactions.FindAsync(id);
        if (transaction is null)
        {
            return false;
        }

        _db.Transactions.Remove(transaction);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<TransactionResponseDto>> GetAllAsync(TransactionFilter filter)
    {
        var query = _db.Transactions.AsNoTracking();

        if (filter.Type.HasValue)
        {
            query = query.Where(t => t.Type == filter.Type.Value);
        }

        if (filter.From.HasValue)
        {
            var from = StartOfDay(filter.From.Value);
            query = query.Where(t => t.Timestamp >= from);
        }

        if (filter.To.HasValue)
        {
            // Add one day to include the end date
            var to = StartOfDay(filter.To.Value.AddDays(1));
            query = query.Where(t => t.Timestamp < to);
        }

        if (!string.IsNullOrEmpty(filter.Search))
        {
            // string.Contains is case-sensitive on SQLite, use LIKE for a case-insensitive "contains"
            var pattern = $"%{filter.Search}%";
            query = query.Where(t => EF.Functions.Like(t.Description, pattern));
        }

        if (filter.PageSize > 0)
        {
            query = query
                .OrderByDescending(t => t.Timestamp)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        return await query
            .OrderByDescending(t => t.Timestamp)
            .Select(t => new TransactionResponseDto(
                t.Id,
                t.Timestamp,
                t.Type,
                t.Description,
                t.Amount
            ))
            .ToListAsync();
    }

    public async Task<TransactionResponseDto?> GetByIdAsync(Guid id)
    {
        return await _db
            .Transactions.AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TransactionResponseDto(
                t.Id,
                t.Timestamp,
                t.Type,
                t.Description,
                t.Amount
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<TransactionResponseDto>> GetInRangeAsync(
        DateOnly start,
        DateOnly end
    )
    {
        var from = StartOfDay(start);
        var to = StartOfDay(end.AddDays(1)); // Add one day to include the end date

        return await _db
            .Transactions.AsNoTracking()
            .Where(t => t.Timestamp >= from && t.Timestamp < to)
            .OrderBy(t => t.Timestamp)
            .Select(t => new TransactionResponseDto(
                t.Id,
                t.Timestamp,
                t.Type,
                t.Description,
                t.Amount
            ))
            .ToListAsync();
    }

    public async Task<TransactionResponseDto?> UpdateAsync(Guid id, UpdateTransactionDto dto)
    {
        var transaction = await _db.Transactions.FindAsync(id);
        if (transaction is null)
        {
            return null;
        }

        transaction.Timestamp = dto.Timestamp!.Value;
        transaction.Type = dto.Type!.Value;
        transaction.Description = dto.Description;
        transaction.Amount = dto.Amount;

        await _db.SaveChangesAsync();
        return ToDto(transaction);
    }

    private static TransactionResponseDto ToDto(Transaction t) =>
        new(t.Id, t.Timestamp, t.Type, t.Description, t.Amount);

    // The database compares points in time, so a calendar day starts at midnight UTC.
    private static DateTimeOffset StartOfDay(DateOnly date) =>
        new(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
}
