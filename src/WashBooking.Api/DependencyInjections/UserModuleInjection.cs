using Modules.Users.Application;
using Modules.Users.Infrastructure;
using Modules.Users.Presentation;

namespace WashBooking.Api.DependencyInjections;

internal static class UserModuleInjection
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPresentation();

        services.AddApplication();

        services.AddInfrastructure(configuration);
        
        return services;
    }
}