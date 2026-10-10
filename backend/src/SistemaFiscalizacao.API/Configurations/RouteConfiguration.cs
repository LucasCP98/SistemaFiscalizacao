namespace SistemaFiscalizacao.API.Configurations
{
    public static class RouteConfig
    {
        public static IServiceCollection AddRouteConfig(
            this IServiceCollection service)
        {
            service.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;

            });
            return service;
        }
    }
}
