using System.ComponentModel.DataAnnotations;

namespace BudgetApi.Dtos.Auth;

public record RegisterDto(
    [Required, StringLength(100, MinimumLength = 1)] string Name,
    [Required, EmailAddress, StringLength(256)] string Email,
    [Required, MinLength(8)] string Password
);
