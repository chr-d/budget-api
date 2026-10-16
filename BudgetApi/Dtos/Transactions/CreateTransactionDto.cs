using BudgetApi.Models;

namespace BudgetApi.Dtos.Transactions;

public record CreateTransactionDto(
    DateTimeOffset? Timestamp,
    TransactionType? Type,
    string Description,
    decimal Amount
);
