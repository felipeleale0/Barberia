using Barberia.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barberia.Data.Configurations
{
    public class EstadoConfiguration : IEntityTypeConfiguration<Estado>
    {
        public void Configure(EntityTypeBuilder<Estado> builder)
        {
            builder.HasData(
                new Estado { Id = 1, Descripcion = "Pendiente" },
                new Estado { Id = 2, Descripcion = "Confirmado" },
                new Estado { Id = 3, Descripcion = "Cancelado" },
                new Estado { Id = 4, Descripcion = "Reservado" }

            );
        }
    }
}
