using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Application;
using Modules.Users.Application.Services;
using Modules.Users.Domain.Interfaces;
using Modules.Users.Infrastructure.Persistence.DBContext;
using Modules.Users.Infrastructure.Persistence.Repositories;
using Modules.Users.Presentation;

namespace Modules.Users.Infrastructure;

public static class UserModuleInjection
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPresentation();
        // 1. Kêu tầng Application tự đăng ký đồ chơi của nó (MediatR, FluentValidation...)
        services.AddApplication();
        
        var connectionString = configuration.GetConnectionString("WashBookingDb");
        
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Mốt ông có Repository hay Service của User thì đăng ký hết ở đây nha!
        services.AddScoped<IUsersUnitOfWork>(sp => sp.GetRequiredService<UserDbContext>());
        services.AddScoped<IUserRepository, UserReposity>();
        
        return services;
    }
}