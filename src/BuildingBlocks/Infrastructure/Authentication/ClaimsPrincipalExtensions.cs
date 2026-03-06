using System.Security.Claims;

namespace Infrastructure.Authentication;


internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            userId = principal?.FindFirst("sub")?.Value;
        }

        return Guid.TryParse(userId, out Guid parsedUserId) 
            ? parsedUserId 
            : throw new InvalidOperationException("User identifier is missing or invalid in the current context.");
    }
}
