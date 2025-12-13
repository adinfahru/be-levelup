using System.Security.Claims;

namespace LevelUp.API.Utilities;

public static class ClaimsExtensions
{
    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Email)
            ?? throw new UnauthorizedAccessException("Email claim not found");
    }
}