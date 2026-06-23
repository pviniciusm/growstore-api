using GrowStore.Domain.Interfaces;
using GrowStore.Infrastructure.Repositories;

namespace GrowStore.API.Config
{
    public static class RepositoryConfig
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, MockUserRepository>();

            return services;
        }
    }
}
