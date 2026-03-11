using Microsoft.EntityFrameworkCore;
using UserManagementApi.Application.Interfaces;
using UserManagementApi.Application.Services;
using UserManagementApi.Domain.Interfaces;
using UserManagementApi.Infrastructure.Messaging;
using UserManagementApi.Infrastructure.Persistence;
using UserManagementApi.Infrastructure.Persistence.Repositories;

namespace UserManagementApi.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMessagePublisher, MessagePublisher>();

            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
