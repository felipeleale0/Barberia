using Microsoft.EntityFrameworkCore;
using Barberia.Models;

namespace Barberia.Data
{
    public class BarberiaContext : DbContext
    {
        public BarberiaContext(DbContextOptions<BarberiaContext> options)
            : base(options)
        {
        }

        public DbSet<Barbero> Barberos { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<BarberoServicio> BarberoServicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relación muchos a muchos entre Barbero y Servicio
            modelBuilder.Entity<BarberoServicio>()
                .HasKey(bs => new { bs.BarberoId, bs.ServicioId });

            modelBuilder.Entity<BarberoServicio>()
                .HasOne(bs => bs.Barbero)
                .WithMany(b => b.BarberoServicios)
                .HasForeignKey(bs => bs.BarberoId);

            modelBuilder.Entity<BarberoServicio>()
                .HasOne(bs => bs.Servicio)
                .WithMany(s => s.BarberoServicios)
                .HasForeignKey(bs => bs.ServicioId);

            // Ignorar tipos del framework MVC que EF puede confundir como entidades
            modelBuilder.Ignore<Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource>();
            modelBuilder.Ignore<Microsoft.AspNetCore.Mvc.ViewFeatures.ModelExplorer>();
            modelBuilder.Ignore<Microsoft.AspNetCore.Mvc.Filters.FilterDescriptor>();

            base.OnModelCreating(modelBuilder);
        }
    }
}

