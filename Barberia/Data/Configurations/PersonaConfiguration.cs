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
        }
    }
}
