using ManageUsers.Application;
using ManageUsers.Application.Common;
using ManageUsers.Application.Interfaces;
using ManageUsers.Infrastructure.Persistence;
using ManageUsers.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManageUsers.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("UserManagementConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
