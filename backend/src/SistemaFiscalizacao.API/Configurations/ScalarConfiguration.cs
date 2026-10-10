using Scalar.AspNetCore;

namespace SistemaFiscalizacao.API.Configurations
{
    public static class ScalarConfiguration
    {
        private static readonly string AppName = "Sistema de Fiscalização - API Principal";

        public static WebApplication UseScalarConfiguration(
            this WebApplication app)
        {
            app.MapScalarApiReference("/scalar", options =>
            {
                options
                .WithTitle(AppName)
                .WithOpenApiRoutePattern("/swagger/v1/swagger.json");
            });
            return app;
        }
    }
}
