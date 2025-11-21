using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barberia.Models.Domain
{
    public class Servicio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public string? Descripcion { get; set; }

        // Barberos que ofrecen este servicio
        public List<BarberoServicio> BarberoServicios { get; set; } = new();

        // Reservas que eligieron este servicio
        public List<Reserva> Reservas { get; set; } = new();
    }
}
