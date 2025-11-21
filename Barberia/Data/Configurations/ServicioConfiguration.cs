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
            var servicios = new List<Servicio>
            {
                new Servicio
                {
                    Id = 1,
                    Nombre = "Corte clásico",
                    Precio = 5000m,
                    Descripcion = "Corte tradicional con tijera y máquina."
                },
                new Servicio
                {
                    Id = 2,
                    Nombre = "Corte degradado (Fade)",
                    Precio = 6500m,
                    Descripcion = "Fade bajo, medio o alto, con terminaciones a navaja."
                },
                new Servicio
                {
                    Id = 3,
                    Nombre = "Arreglo de barba",
                    Precio = 3500m,
                    Descripcion = "Perfilado, rebaje y prolijo general."
                },
                new Servicio
                {
                    Id = 4,
                    Nombre = "Afeitado clásico",
                    Precio = 4500m,
                    Descripcion = "Afeitado con toalla caliente y navaja."
                },
                new Servicio
                {
                    Id = 5,
                    Nombre = "Tintura para cabello",
                    Precio = 9000m,
                    Descripcion = "Coloración tradicional para cabello."
                },
                new Servicio
                {
                    Id = 6,
                    Nombre = "Tintura para barba",
                    Precio = 6000m,
                    Descripcion = "Coloración y perfilado de barba."
                }
            };

            builder.HasData(servicios);
        }
    }
}
