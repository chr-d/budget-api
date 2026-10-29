using Microsoft.AspNetCore.Identity;

namespace BudgetApi.Models;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    public List<Transaction> Transactions { get; set; } = [];
}
