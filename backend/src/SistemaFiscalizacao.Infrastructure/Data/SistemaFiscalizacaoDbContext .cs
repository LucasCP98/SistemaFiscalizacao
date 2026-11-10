
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
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<Regularizacao> Regularizacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relacionamento: Vistoria -> Denuncia
            modelBuilder.Entity<Vistoria>()
                .HasOne(v => v.Denuncia)
                .WithMany()
                .HasForeignKey(v => v.DenunciaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento: Notificacao -> Denuncia
            modelBuilder.Entity<Notificacao>()
                .HasOne(n => n.Denuncia)
                .WithMany()
                .HasForeignKey(n => n.DenunciaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento: Notificacao -> Vistoria
            modelBuilder.Entity<Notificacao>()
                .HasOne(n => n.Vistoria)
                .WithMany()
                .HasForeignKey(n => n.VistoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento: Regularizacao -> Notificacao
            modelBuilder.Entity<Regularizacao>()
                .HasOne(r => r.Notificacao)
                .WithMany()
                .HasForeignKey(r => r.NotificacaoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
