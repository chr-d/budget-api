using BudgetApi.Models;

namespace BudgetApi.Dtos.Transactions;

public record TransactionFilter(
    TransactionType? Type,
    DateOnly? From,
    DateOnly? To,
    string? Search,
    int Page,
    int PageSize
);
