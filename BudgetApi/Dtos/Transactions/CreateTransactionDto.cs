using System.ComponentModel.DataAnnotations;
using BudgetApi.Models;

namespace BudgetApi.Dtos.Transactions;

public record CreateTransactionDto(
    DateTimeOffset? Timestamp,
    [Required] TransactionType? Type,
    [Required, StringLength(200, MinimumLength = 1)] string Description,
    [Required, Range(0.01, (double)decimal.MaxValue)] decimal Amount
);
