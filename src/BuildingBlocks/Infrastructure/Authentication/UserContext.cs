using BuildingBlocks.Application.Abstractions.Authentication; 
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Infrastructure.Authentication;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?.User.GetUserId();

            if (userId is null || userId == Guid.Empty)
            {
                throw new InvalidOperationException("User context is unavailable. Are you authenticated?");
            }

            return userId.Value;
        }
    }
}
