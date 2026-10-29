using BudgetApi.Dtos.Auth;
using BudgetApi.Dtos.Users;
using Microsoft.AspNetCore.Identity;

namespace BudgetApi.Services;

public interface IAuthService
{
    Task<(IdentityResult Result, UserResponseDto? User)> RegisterAsync(RegisterDto dto);
}
