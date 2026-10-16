using Microsoft.AspNetCore.Diagnostics;

namespace BlogApi.Errors;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService
    )
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var (statusCode, title) = exception switch
        {
            BadHttpRequestException badRequestException => (
                badRequestException.StatusCode,
                "Bad Request"
            ),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                $"Unhandled exception occurred while processing request: {context.Request.Method} {context.Request.Path}"
            );
        }

        context.Response.StatusCode = statusCode;

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = context,
                Exception = exception,
                ProblemDetails =
                {
                    Status = statusCode,
                    Title = title,
                    Detail =
                        statusCode == StatusCodes.Status500InternalServerError
                            ? "An unexpected error occurred."
                            : exception.Message,
                },
            }
        );
    }
}
