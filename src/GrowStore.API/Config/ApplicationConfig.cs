using GrowStore.Application.Users.Interfaces;
using GrowStore.Application.Users.Services;

namespace GrowStore.API.Config
{
    public static class ApplicationConfig
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
