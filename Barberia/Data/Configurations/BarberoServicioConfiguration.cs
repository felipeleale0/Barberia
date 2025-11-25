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

            builder.HasData(
                // Juan (Empleado 2)
                new BarberoServicio { EmpleadoId = 2, ServicioId = 1 },
                new BarberoServicio { EmpleadoId = 2, ServicioId = 2 },
                new BarberoServicio { EmpleadoId = 2, ServicioId = 3 },

                // Mario (Empleado 3)
                new BarberoServicio { EmpleadoId = 3, ServicioId = 3 },
                new BarberoServicio { EmpleadoId = 3, ServicioId = 4 },
                new BarberoServicio { EmpleadoId = 3, ServicioId = 1 },

                // Luis (Empleado 4)
                new BarberoServicio { EmpleadoId = 4, ServicioId = 5 },
                new BarberoServicio { EmpleadoId = 4, ServicioId = 6 },
                new BarberoServicio { EmpleadoId = 4, ServicioId = 1 }
            );
        }
    }
}
