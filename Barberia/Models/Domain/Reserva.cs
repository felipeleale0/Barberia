using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barberia.Models.Domain
{
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TurnoId { get; set; }
        public Turno Turno { get; set; } = null!;

        [Required]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        [Required]
        public int EstadoId { get; set; }
        public Estado Estado { get; set; } = null!;

        // Servicio elegido para este turno
        [Required]
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; } = null!;

        // Costo total de la reserva (lo copiás del precio del servicio al crear la reserva)
        [Column(TypeName = "decimal(10,2)")]
        public decimal CostoTotal { get; set; }
    }
}
