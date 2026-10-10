using Microsoft.OpenApi;

namespace SistemaFiscalizacao.API.Configurations
{
    public static class OpenApiConfiguration
    {
        private static readonly string AppName = "Sistema de Fiscalização - Core API";
        private static readonly string AppDescription = "API RESTful desenvolvida em .NET 10 integrada a uma arquitetura de microsserviços/BFF. Responsável pelas regras de domínio e persistência em banco de dados MSSQL, projetada para suportar conteinerização com Docker e orquestração via Kubernetes.";

        public static IServiceCollection AddOpenAPIConfig(this IServiceCollection services)
        {
            services.AddSingleton(new OpenApiInfo
            {
                Title = AppName,
                Version = "v1",
                Description = AppDescription,
                Contact = new OpenApiContact
                {
                    Name = "Lucas Costa",
                    Url = new Uri("https://github.com/LucasCP98")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://github.com/LucasCP98")
                }
            });
            return services;
        }
    }
}

