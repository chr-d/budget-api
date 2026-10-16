using BudgetApi.Dtos.Transactions;

namespace BudgetApi.Services;

public class TransactionService : ITransactionService
{
    public Task<TransactionResponseDto> CreateAsync(CreateTransactionDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<TransactionResponseDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TransactionResponseDto?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<TransactionResponseDto>> GetInRangeAsync(DateOnly start, DateOnly end)
    {
        throw new NotImplementedException();
    }

    public Task<TransactionResponseDto?> UpdateAsync(Guid id, UpdateTransactionDto dto)
    {
        throw new NotImplementedException();
    }
}
