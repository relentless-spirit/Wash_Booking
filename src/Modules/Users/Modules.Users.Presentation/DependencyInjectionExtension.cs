using Microsoft.Extensions.DependencyInjection;

namespace Modules.Users.Presentation;

public static class DependencyInjectionExtension
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        return services;
    }
}