using BudgetApi.Models;

namespace BudgetApi.Dtos.Transactions;

public record UpdateTransactionDto(
    DateTimeOffset? Timestamp,
    TransactionType? Type,
    string Description,
    decimal Amount
);
