using System.ComponentModel.DataAnnotations;

namespace Barberia.Models.Domain
{
    public class Turno
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } // Podés usar solo la parte de fecha

        [Required]
        public TimeSpan Hora { get; set; } // Horario del turno

        [Required]
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; } = null!;

        public bool EstaDisponible { get; set; } = true;

        public List<Reserva> Reservas { get; set; } = new();
    }
}
