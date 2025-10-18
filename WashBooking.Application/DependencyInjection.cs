using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WashBooking.Application.Interfaces;
using WashBooking.Application.Interfaces.Auth;
using WashBooking.Application.Interfaces.Booking;
using WashBooking.Application.Services;
using WashBooking.Application.Services.Auth;

namespace WashBooking.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Add application services here, e.g.:
            services.AddScoped<ICreateBookingService, CreateBookingService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingDetailService, BookingDetailService>();
            //services.AddScoped<IOauthAccountService, OauthAccountService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            //services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserAuthenticator, UserAuthenticator>();
            services.AddScoped<ITokenOrchestrator, TokenOrchestrator>();

            // Add other services as needed
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
