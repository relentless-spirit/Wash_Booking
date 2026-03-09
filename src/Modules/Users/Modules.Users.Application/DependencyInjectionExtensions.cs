using Microsoft.Extensions.DependencyInjection;

namespace Modules.Users.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR and FluentValidation are registered centrally by BuildingBlocks.AddBuildingBlocks()
        // Add module-specific application services here if needed
        
        return services;
    }
}