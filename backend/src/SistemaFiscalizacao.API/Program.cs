using SistemaFiscalizacao.API.Configurations;
using SistemaFiscalizacao.Application.Services;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Infrastructure.Repositories;
using SistemaFiscalizacao.Application.Services.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Configuração de logs
builder.AddSerilogLogging();

builder.Services.AddControllers().AddContentNegotiation();

builder.Services.AddAuthorization();

// Configurações do Swagger, OpenAPI e Rotas
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenAPIConfig();
builder.Services.AddSwaggerConfig();
builder.Services.AddRouteConfig();

// Configuração do Banco de Dados
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// Injeção de Dependência dos seus Serviços e Repositórios
builder.Services.AddScoped<IDenunciaService, DenunciaServiceImplementation>();
builder.Services.AddScoped<IRepository<Denuncia>, DenunciaRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Ativação das interfaces visuais da API
app.UseSwaggerConfiguration();
app.UseScalarConfiguration();

app.Run();
