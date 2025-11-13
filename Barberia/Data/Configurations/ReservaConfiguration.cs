using Barberia.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barberia.Data.Configurations
{
    public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
    {
        public void Configure(EntityTypeBuilder<Reserva> builder)
        {
            builder.Property(r => r.CostoTotal)
                .HasColumnType("decimal(10,2)");

            builder.HasOne(r => r.Turno)
                .WithMany(t => t.Reservas)
                .HasForeignKey(r => r.TurnoId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Usuario)
                .WithMany(u => u.Reservas)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Estado)
                .WithMany(e => e.Reservas)
                .HasForeignKey(r => r.EstadoId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Servicio)
                .WithMany(s => s.Reservas)
                .HasForeignKey(r => r.ServicioId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
