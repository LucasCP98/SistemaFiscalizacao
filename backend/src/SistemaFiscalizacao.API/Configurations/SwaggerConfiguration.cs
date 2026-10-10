using Microsoft.OpenApi;

namespace SistemaFiscalizacao.API.Configurations
{
    public static class SwaggerConfiguration
    {
        private static readonly string AppName = "Sistema de Fiscalização - API Principal";
        private static readonly string AppDescription = "API RESTful responsável pelo processamento de dados fiscais, regras de negócio e persistência do Sistema de Fiscalização. Atua como o Core Backend, fornecendo dados de forma segura para a camada de BFF (Backend For Frontend).";

        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
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
                options.CustomSchemaIds(type => type.FullName);
            });
            return services;
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = "swagger-ui";
                options.DocumentTitle = AppName;
            });
            return app;
        }
    }
}
