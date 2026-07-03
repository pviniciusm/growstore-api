using FluentValidation;
using GrowStore.Application.Auth.Interfaces;
using GrowStore.Application.Auth.Services;
using GrowStore.Application.Carts.Interfaces;
using GrowStore.Application.Carts.Services;
using GrowStore.Application.Categories.Interfaces;
using GrowStore.Application.Categories.Services;
using GrowStore.Application.Addresses.Interfaces;
using GrowStore.Application.Addresses.Services;
using GrowStore.Application.Products.Interfaces;
using GrowStore.Application.Products.Services;
using GrowStore.Application.Users.Interfaces;
using GrowStore.Application.Users.Services;
using GrowStore.Application.Users.Validators;
using GrowStore.Application.Orders.Interfaces;
using GrowStore.Application.Orders.Services;

namespace GrowStore.API.Config
{
    public static class ApplicationConfig
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();

            // Automatically registers all validators from the Application assembly
            services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
            services.AddScoped<IOrderService, OrderService>();



            return services;
        }
    }
}


