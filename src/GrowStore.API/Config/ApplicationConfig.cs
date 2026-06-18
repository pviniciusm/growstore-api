using GrowStore.Application.Auth.Interfaces;
using GrowStore.Application.Auth.Services;
using GrowStore.Application.Users.Interfaces;
using GrowStore.Application.Users.Services;

namespace GrowStore.API.Config
{
    public static class ApplicationConfig
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
             services.AddScoped<IAuthService, MockAuthService>();

            return services;
        }
    }
}
