using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Barberia.Models
{
    public class Barbero
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Especialidad { get; set; }

        public int AñosExperiencia { get; set; }

        // Relación muchos a muchos con Servicio
        public List<BarberoServicio> BarberoServicios { get; set; } = new();
    }
}
