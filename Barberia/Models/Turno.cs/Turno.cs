using Barberia.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barberia.Models
{
    public class Turno
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Cliente { get; set; }

        // Relación con Barbero
        [ForeignKey("Barbero")]
        public int BarberoId { get; set; }
        public Barbero Barbero { get; set; }

        // Relación con Servicio
        [ForeignKey("Servicio")]
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        public decimal Precio { get; set; }
    }
}

