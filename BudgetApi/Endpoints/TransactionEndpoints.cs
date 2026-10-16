using BudgetApi.Dtos.Transactions;
using BudgetApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BudgetApi.Endpoints;

public static class TransactionEndpoints
{
    public static RouteGroupBuilder MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/transactions").AddEndpointFilter<RejectEmptyIdFilter>();
        group.MapGet("/", GetAllTransactions);
        group.MapGet("/{id:Guid}", GetTransactionById);
        group.MapPost("/", CreateTransaction);
        group.MapPut("/{id:Guid}", UpdateTransaction);
        group.MapDelete("/{id:Guid}", DeleteTransaction);

        return group;
    }

    private static async Task<Ok<IReadOnlyList<TransactionResponseDto>>> GetAllTransactions(
        ITransactionService transactionService
    )
    {
        return TypedResults.Ok(await transactionService.GetAllAsync());
    }

    private static async Task<Results<Ok<TransactionResponseDto>, NotFound>> GetTransactionById(
        Guid id,
        ITransactionService transactionService
    )
    {
        var transaction = await transactionService.GetByIdAsync(id);
        return transaction is null ? TypedResults.NotFound() : TypedResults.Ok(transaction);
    }

    private static async Task<Created<TransactionResponseDto>> CreateTransaction(
        CreateTransactionDto dto,
        ITransactionService transactionService
    )
    {
        var transaction = await transactionService.CreateAsync(dto);

        //TODO: make the path dynamic
        return TypedResults.Created($"/api/transactions/{transaction.Id}", transaction);
    }

    private static async Task<Results<Ok<TransactionResponseDto>, NotFound>> UpdateTransaction(
        Guid id,
        UpdateTransactionDto dto,
        ITransactionService transactionService
    )
    {
        var transaction = await transactionService.UpdateAsync(id, dto);
        return transaction is null ? TypedResults.NotFound() : TypedResults.Ok(transaction);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteTransaction(
        Guid id,
        ITransactionService transactionService
    )
    {
        return await transactionService.DeleteAsync(id)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
}
