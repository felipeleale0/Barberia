using Microsoft.EntityFrameworkCore;
using Barberia.Models.Domain;

namespace Barberia.Data
{
    public class BarberiaContext : DbContext
    {
        public BarberiaContext(DbContextOptions<BarberiaContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<BarberoServicio> BarberoServicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica todas las configuraciones del assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BarberiaContext).Assembly);

            // Si seguís necesitando ignorar tipos de MVC:
            // modelBuilder.Ignore<Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource>();
            // modelBuilder.Ignore<Microsoft.AspNetCore.Mvc.ViewFeatures.ModelExplorer>();
            // modelBuilder.Ignore<Microsoft.AspNetCore.Mvc.Filters.FilterDescriptor>();

            base.OnModelCreating(modelBuilder);
        }
    }
}