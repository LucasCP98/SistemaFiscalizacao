
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
        public DbSet<ProcessoInfracao> ProcessosInfracao { get; set; }
        public DbSet<Multa> Multas { get; set; }
        public DbSet<Recurso> Recursos { get; set; }
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

            // Relacionamento: ProcessoInfracao -> Denuncia
            modelBuilder.Entity<ProcessoInfracao>()
                .HasOne(p => p.Denuncia)
                .WithMany()
                .HasForeignKey(p => p.DenunciaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento: ProcessoInfracao -> Vistoria
            modelBuilder.Entity<ProcessoInfracao>()
                .HasOne(p => p.Vistoria)
                .WithMany()
                .HasForeignKey(p => p.VistoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice único: um processo de infração por denúncia
            modelBuilder.Entity<ProcessoInfracao>()
                .HasIndex(p => p.DenunciaId)
                .IsUnique();

            // Precisão do valor da multa
            modelBuilder.Entity<ProcessoInfracao>()
                .Property(p => p.ValorMulta)
                .HasPrecision(18, 2);

            // Relacionamento: Multa -> ProcessoInfracao
            modelBuilder.Entity<Multa>()
                .HasOne(m => m.ProcessoInfracao)
                .WithOne(p => p.Multa)
                .HasForeignKey<Multa>(m => m.ProcessoInfracaoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Precisão dos valores monetários da multa
            modelBuilder.Entity<Multa>()
                .Property(m => m.Valor)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Multa>()
                .Property(m => m.ValorMinimo)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Multa>()
                .Property(m => m.ValorMaximo)
                .HasPrecision(18, 2);

            // Relacionamento: Recurso -> ProcessoInfracao
            modelBuilder.Entity<Recurso>()
                .HasOne(r => r.ProcessoInfracao)
                .WithMany(p => p.Recursos)
                .HasForeignKey(r => r.ProcessoInfracaoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
