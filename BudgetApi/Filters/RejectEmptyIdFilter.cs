namespace BudgetApi.Filters;

public class RejectEmptyIdFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var routeValues = context.HttpContext.Request.RouteValues;

        if (
            routeValues.TryGetValue("id", out var value)
            && Guid.TryParse(value?.ToString(), out var id)
            && id == Guid.Empty
        )
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]> { ["id"] = ["The id must not be an empty GUID."] }
            );
        }

        return await next(context);
    }
}
