using GrowStore.Application.Auth.Interfaces;
using GrowStore.Application.Auth.Services;
using GrowStore.Application.Users.Interfaces;
using GrowStore.Application.Users.Services;
using GrowStore.Application.Categories.Interfaces;
using GrowStore.Application.Categories.Services;
using GrowStore.Application.Addresses.Interfaces;
using GrowStore.Application.Addresses.Services;
namespace GrowStore.API.Config
{
    public static class ApplicationConfig
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAddressService, AddressService>();
            return services;
        }
    }
}
