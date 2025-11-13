using System.ComponentModel.DataAnnotations;

namespace Barberia.Models.Domain
{
    public class Estado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Descripcion { get; set; } = null!; // Ej: Pendiente, Confirmado, Cancelado

        public List<Reserva> Reservas { get; set; } = new();
    }
}
