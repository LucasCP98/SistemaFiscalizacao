using SistemaFiscalizacao.API.Configurations;
using SistemaFiscalizacao.Application.Services;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Infrastructure.Repositories;
using SistemaFiscalizacao.Application.Services.Implementation;



var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddControllers().AddContentNegotiation();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenAPIConfig();

builder.Services.AddSwaggerConfig();

builder.Services.AddRouteConfig();

builder.Services.AddDatabaseConfiguration(builder.Configuration);

//builder.Services.AddEvolveConfiguration(builder.Configuration, builder.Environment);

builder.Services.AddScoped<IDenunciaService, DenunciaServiceImplementation>();

builder.Services.AddScoped<IRepository<Denuncia>, DenunciaRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwaggerConfiguration();

app.UseScalarConfiguration();

app.Run();
