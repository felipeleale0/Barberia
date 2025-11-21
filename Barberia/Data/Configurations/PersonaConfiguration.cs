using Barberia.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barberia.Data.Configurations
{
    public class PersonaConfiguration : IEntityTypeConfiguration<Persona>
    {
        public void Configure(EntityTypeBuilder<Persona> builder)
        {
            builder.HasOne(p => p.Usuario)
                .WithOne(u => u.Persona)
                .HasForeignKey<Persona>(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(p => p.CreatedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("GETDATE()");

            builder.HasQueryFilter(p => !p.Usuario.EstaEliminado);

            builder.HasData(
                new Persona { Id = 1, Nombre = "Juan", Apellido = "Gómez", CorreoElectronico = "juan@barberia.local", EsBarbero = true, UsuarioId = 2 },
                new Persona { Id = 2, Nombre = "Mario", Apellido = "Pérez", CorreoElectronico = "mario@barberia.local", EsBarbero = true, UsuarioId = 3 },
                new Persona { Id = 3, Nombre = "Luis", Apellido = "Rodríguez", CorreoElectronico = "luis@barberia.local", EsBarbero = true, UsuarioId = 4 }
            );
        }
    }
}
