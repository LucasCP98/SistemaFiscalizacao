
using SistemaFiscalizacao.API.Configurations;
using SistemaFiscalizacao.Application.Services;
using SistemaFiscalizacao.Application.Services.Implementation;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Infrastructure.Repositories;
using SistemaFiscalizacao.Infrastructure.Transactions;

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

// Injeção de Dependência dos Serviços
builder.Services.AddScoped<IDenunciaService, DenunciaServiceImplementation>();
builder.Services.AddScoped<IVistoriaService, VistoriaServiceImplementation>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
builder.Services.AddScoped<INotificacaoService, NotificacaoServiceImplementation>();
builder.Services.AddScoped<IRegularizacaoService, RegularizacaoServiceImplementation>();
builder.Services.AddScoped<IProcessoInfracaoService, ProcessoInfracaoServiceImplementation>();
builder.Services.AddScoped<IMultaService, MultaServiceImplementation>();

// Injeção de Dependência do Repositório Genérico
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Ativação das interfaces visuais da API
app.UseSwaggerConfiguration();
app.UseScalarConfiguration();

app.Run();
