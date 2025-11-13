using Barberia.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barberia.Data.Configurations
{
    public class BarberoServicioConfiguration : IEntityTypeConfiguration<BarberoServicio>
    {
        public void Configure(EntityTypeBuilder<BarberoServicio> builder)
        {
            builder.HasKey(bs => new { bs.EmpleadoId, bs.ServicioId });

            builder.HasOne(bs => bs.Empleado)
                .WithMany(e => e.BarberoServicios)
                .HasForeignKey(bs => bs.EmpleadoId);

            builder.HasOne(bs => bs.Servicio)
                .WithMany(s => s.BarberoServicios)
                .HasForeignKey(bs => bs.ServicioId);
        }
    }
}
