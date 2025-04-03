
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TextProcessor.Data
{
    public static class DbConfigurationDI
    {
        public static IServiceCollection AddDbService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            string connectionStringName = "DbConnectionString";
            string? connectionString = configuration.GetConnectionString(connectionStringName);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception($"Connection string [{connectionStringName}] is empty. Check 'appsettings.json' in root");

            services.AddDbContext<TextProcessDbContext>(optionsBuilder => {
                optionsBuilder.UseSqlServer(connectionString);
            });
            return services;
        }
    }
}
