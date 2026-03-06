using System.Security.Claims;

namespace Infrastructure.Authentication;


internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        // 1. Ưu tiên tìm NameIdentifier
        string? userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // 2. Nếu không thấy, tìm tiếp "sub" (Subject - chuẩn JWT quốc tế)
        if (string.IsNullOrEmpty(userId))
        {
            userId = principal?.FindFirst("sub")?.Value;
        }

        // 3. Parse Guid
        return Guid.TryParse(userId, out Guid parsedUserId) 
            ? parsedUserId 
            : throw new InvalidOperationException("User identifier is missing or invalid in the current context.");
        // Dùng InvalidOperationException đúng ngữ nghĩa hơn ApplicationException
    }
}