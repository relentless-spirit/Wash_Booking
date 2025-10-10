using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using WashBooking.Common;

namespace WashBooking.Middlewares;

public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly IAuthorizationMiddlewareResultHandler _defaultHandler = new AuthorizationMiddlewareResultHandler();

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, 
        PolicyAuthorizationResult authorizeResult)
    {
        // Nếu authorization thất bại
        if (!authorizeResult.Succeeded)
        {
            context.Response.ContentType = "application/json";
            // Kiểm tra xem user đã authenticate chưa
            if (authorizeResult.Challenged)
            {
                // Nếu chưa authenticate → 401
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse("Auth.Unauthorized",
                    "Authentication required. Please provide a valid token")));
                return;
            }
            
            // Nếu đã authenticate nhưng không có quyền → 403  
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse("Auth.Forbidden",
                "Access denied. You do not have permission to perform this action.")));
            return;
        }

        // Nếu authorization thành công, tiếp tục với request
        await next(context);
    }
}