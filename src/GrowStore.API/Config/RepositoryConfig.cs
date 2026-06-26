using GrowStore.Domain.Interfaces;
using GrowStore.Infrastructure.Data;
using GrowStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GrowStore.API.Config
{
    public static class RepositoryConfig
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<GrowStoreDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();

            return services;
        }
    }
}