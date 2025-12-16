using System.Security.Claims;

namespace LevelUp.API.Utilities;

public static class ClaimsExtensions
{
    public static Guid GetAccountId(this ClaimsPrincipal user)
    {
        var accountId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(accountId))
            throw new UnauthorizedAccessException("AccountId claim not found");

        return Guid.Parse(accountId);
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Email)
            ?? throw new UnauthorizedAccessException("Email claim not found");
    }
}