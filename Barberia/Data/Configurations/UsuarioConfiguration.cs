using Barberia.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barberia.Data.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            // Tipo y default de fecha
            builder.Property(u => u.CreatedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("GETDATE()");

            // Defaults para flags
            builder.Property(u => u.EstaBloqueado)
                   .HasDefaultValue(false);

            builder.Property(u => u.EstaEliminado)
                   .HasDefaultValue(false);

            // (Opcional pero muy útil) filtro global de soft delete
            builder.HasQueryFilter(u => !u.EstaEliminado);
        }
    }
}
