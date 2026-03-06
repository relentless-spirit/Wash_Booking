using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.Users.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjectionExtensions).Assembly;

        services.AddMediatR(config => { config.RegisterServicesFromAssembly(assembly); });

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        return services;
    }
}