using Microsoft.EntityFrameworkCore;
using SistemaFiscalizacao.Infrastructure.Data;

namespace SistemaFiscalizacao.API.Configurations
{
    public static class DataBaseConfiguration
    {
        public static IServiceCollection AddDatabaseConfiguration(
            this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");
            }

            services.AddDbContext<SistemaFiscalizacaoDbContext>(options => options.UseSqlServer(connectionString));
            return services;

        }
    }
}
