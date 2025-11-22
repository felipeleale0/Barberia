using Barberia.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barberia.Data.Configurations
{
    public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
    {
        public void Configure(EntityTypeBuilder<Empleado> builder)
        {
            builder.HasOne(e => e.Persona)
                .WithOne(p => p.Empleado)
                .HasForeignKey<Empleado>(e => e.PersonaId);

            builder.HasMany(e => e.Turnos)
                .WithOne(t => t.Empleado)
                .HasForeignKey(t => t.EmpleadoId);

            builder.HasData(
                new Empleado { Id = 2, PersonaId = 1 },
                new Empleado { Id = 3, PersonaId = 2 },
                new Empleado { Id = 4, PersonaId = 3 }
            );
        }
    }
}
