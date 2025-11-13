using Barberia.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barberia.Data.Configurations
{
    public class ServicioConfiguration : IEntityTypeConfiguration<Servicio>
    {
        public void Configure(EntityTypeBuilder<Servicio> builder)
        {
            builder.Property(s => s.Precio)
                .HasColumnType("decimal(10,2)");

            // Seed: un corte básico
            builder.HasData(
                new Servicio
                {
                    Id = 1,
                    Nombre = "Corte clásico",
                    Precio = 2500m,
                    Descripcion = "Corte de cabello estándar."
                }
            );
        }
    }
}
