using GrowStore.Application.Auth.Interfaces;
using GrowStore.Application.Auth.Services;
using GrowStore.Application.Users.Interfaces;
using GrowStore.Application.Users.Services;
using GrowStore.Application.Categories.Interfaces;
using GrowStore.Application.Categories.Services;
using GrowStore.Application.Products.Interfaces;
using GrowStore.Application.Products.Services;

namespace GrowStore.API.Config
{
    public static class ApplicationConfig
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            return services;
        }
    }
}