using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Barberia.Models
{
    public class Servicio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public decimal Precio { get; set; }
        public string Descripcion { get; set; }

        // Relación muchos a muchos con Barbero
        public List<BarberoServicio> BarberoServicios { get; set; } = new();
    }
}
