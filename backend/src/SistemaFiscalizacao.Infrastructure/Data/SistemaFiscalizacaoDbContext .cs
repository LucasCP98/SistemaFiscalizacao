using Microsoft.EntityFrameworkCore;
using SistemaFiscalizacao.Domain.Entities;

namespace SistemaFiscalizacao.Infrastructure.Data
{
    public class SistemaFiscalizacaoDbContext : DbContext
    {
        public SistemaFiscalizacaoDbContext(
            DbContextOptions<SistemaFiscalizacaoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Denuncia> Denuncias { get; set; }
    }
}