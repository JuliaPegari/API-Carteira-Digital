using Microsoft.EntityFrameworkCore;
using CarteiraDigitalAPI.Models;

namespace CarteiraDigitalAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Carteira> Carteiras { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Carteira>()
                .HasOne(c => c.Usuario)
                .WithOne(u => u.Carteira)
                .HasForeignKey<Carteira>(c => c.UsuarioId);

            modelBuilder.Entity<Transacao>()
                .HasOne(t => t.CarteiraOrigem)
                .WithMany(c => c.TransferenciasEnviadas)
                .HasForeignKey(t => t.CarteiraOrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transacao>()
                .HasOne(t => t.CarteiraDestino)
                .WithMany(c => c.TransferenciasRecebidas)
                .HasForeignKey(t => t.CarteiraDestinoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}