using BudgetApi.Dtos.Transactions;

namespace BudgetApi.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionResponseDto>> GetAllAsync(Guid userId, TransactionFilter filter);
    Task<TransactionResponseDto?> GetByIdAsync(Guid userId, Guid id);
    Task<TransactionResponseDto> CreateAsync(Guid userId, CreateTransactionDto dto);
    Task<TransactionResponseDto?> UpdateAsync(Guid userId, Guid id, UpdateTransactionDto dto);
    Task<bool> DeleteAsync(Guid userId, Guid id);
}
