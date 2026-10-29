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
        public DbSet<Vistoria> Vistorias { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vistoria>()
                .HasOne(v => v.Denuncia)
                .WithMany()
                .HasForeignKey(v => v.DenunciaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}