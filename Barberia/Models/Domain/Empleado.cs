using System.ComponentModel.DataAnnotations;

namespace Barberia.Models.Domain
{
    public class Empleado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PersonaId { get; set; }
        public Persona Persona { get; set; } = null!;

        public List<Turno> Turnos { get; set; } = new();

        // Relación muchos a muchos con Servicio
        public List<BarberoServicio> BarberoServicios { get; set; } = new();
    }
}
