using System.ComponentModel.DataAnnotations;
using BudgetApi.Dtos.Transactions;
using BudgetApi.Filters;
using BudgetApi.Models;
using BudgetApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BudgetApi.Endpoints;

public static class TransactionEndpoints
{
    // TODO: remove this when proper authentication and authorization is in place
    private static readonly Guid _devUser = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static RouteGroupBuilder MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/transactions")
            .AddEndpointFilter<RejectEmptyIdFilter>()
            .WithTags("Transactions");
        ;
        group
            .MapGet("/", GetAllTransactions)
            .WithName("GetAllTransactions")
            .WithSummary("Get all transactions")
            .WithDescription(
                "Returns a list of all transactions, newest first. Supports filtering by type, date range, and search term. Supports pagination."
            );
        ;
        group
            .MapGet("/{id:Guid}", GetTransactionById)
            .ProducesValidationProblem()
            .WithName("GetTransactionById")
            .WithSummary("Get a transaction by ID")
            .WithDescription("Returns a single transaction by its ID.");
        group
            .MapPost("/", CreateTransaction)
            .ProducesValidationProblem()
            .WithName("CreateTransaction")
            .WithSummary("Create a new transaction")
            .WithDescription("Creates a new transaction and returns the created transaction.");
        group
            .MapPut("/{id:Guid}", UpdateTransaction)
            .ProducesValidationProblem()
            .WithName("UpdateTransaction")
            .WithSummary("Update a transaction")
            .WithDescription(
                "Updates an existing transaction by its ID and returns the updated transaction."
            );
        group
            .MapDelete("/{id:Guid}", DeleteTransaction)
            .ProducesValidationProblem()
            .WithName("DeleteTransaction")
            .WithSummary("Delete a transaction")
            .WithDescription("Deletes an existing transaction by its ID.");

        return group;
    }

    private static async Task<Ok<IReadOnlyList<TransactionResponseDto>>> GetAllTransactions(
        TransactionType? type,
        DateOnly? from,
        DateOnly? to,
        string? search,
        ITransactionService transactionService,
        [Range(1, int.MaxValue)] int page = 1,
        [Range(1, 100)] int pageSize = 20
    )
    {
        var filter = new TransactionFilter(type, from, to, search, page, pageSize);
        return TypedResults.Ok(await transactionService.GetAllAsync(_devUser, filter));
    }

    private static async Task<Results<Ok<TransactionResponseDto>, NotFound>> GetTransactionById(
        Guid id,
        ITransactionService transactionService
    )
    {
        var transaction = await transactionService.GetByIdAsync(_devUser, id);
        return transaction is null ? TypedResults.NotFound() : TypedResults.Ok(transaction);
    }

    private static async Task<Created<TransactionResponseDto>> CreateTransaction(
        CreateTransactionDto dto,
        ITransactionService transactionService
    )
    {
        var transaction = await transactionService.CreateAsync(_devUser, dto);

        //TODO: make the path dynamic
        return TypedResults.Created($"/api/transactions/{transaction.Id}", transaction);
    }

    private static async Task<Results<Ok<TransactionResponseDto>, NotFound>> UpdateTransaction(
        Guid id,
        UpdateTransactionDto dto,
        ITransactionService transactionService
    )
    {
        var transaction = await transactionService.UpdateAsync(_devUser, id, dto);
        return transaction is null ? TypedResults.NotFound() : TypedResults.Ok(transaction);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteTransaction(
        Guid id,
        ITransactionService transactionService
    )
    {
        return await transactionService.DeleteAsync(_devUser, id)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
}
