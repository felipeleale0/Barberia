using System.ComponentModel.DataAnnotations;
using Barberia.Models.Domain;

namespace Barberia.Models.ViewModels
{
    public class ReservaCreateViewModel
    {
        [Required]
        public int TurnoId { get; set; }
        public Turno Turno { get; set; } = null!;

        [Required]
        [Display(Name = "Servicio")]
        public int ServicioId { get; set; }

        // Lista de servicios que puede hacer el barbero de ese turno
        public List<Servicio> ServiciosDisponibles { get; set; } = new();
    }
}
