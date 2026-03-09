using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application.Services;
using Modules.Users.Domain.Interfaces;
using Modules.Users.Infrastructure.Persistence.DBContext;
using Modules.Users.Infrastructure.Persistence.Repositories;

namespace Modules.Users.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("WashBookingDb");
        
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddScoped<IUsersUnitOfWork>(sp => sp.GetRequiredService<UserDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}