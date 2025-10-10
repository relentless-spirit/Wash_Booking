using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WashBooking.Application.Interfaces;
using WashBooking.Domain.Entities;
using WashBooking.Domain.Interfaces.Persistence;
using WashBooking.Domain.Interfaces.Repositories;
using WashBooking.Infrastructure.Authentication;
using WashBooking.Infrastructure.Persistence;
using WashBooking.Infrastructure.Persistence.Repositories;

namespace WashBooking.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
        {
            // register infrastructure services here
            service.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            service.AddScoped<IAccountRepository, AccountRepository>();
            service.AddScoped<IUserProfileRepository, UserProfileRepository>();
            service.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // register services (unit of work, jwt, auto mapper)
            service.AddScoped<IUnitOfWork, UnitOfWork>();
            service.AddScoped<IJwtProvider, JwtProvider>();

            // register db context
            service.AddDbContext<MotoBikeWashingBookingContext>(option => 
                option.UseNpgsql(configuration.GetConnectionString("DefaultConnectionStringDB")));


            return service;
        }
    }
}
