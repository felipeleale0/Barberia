// Models/ViewModels/TurnoDisponibleViewModel.cs
using System.Collections.Generic;

namespace Barberia.Models.ViewModels
{
    public class TurnoDisponibleViewModel
    {
        public DateTime FechaSeleccionada { get; set; }

        public List<ItemTurno> Turnos { get; set; } = new();

        public class ItemTurno
        {
            public int TurnoId { get; set; }
            public string Hora { get; set; } = null!;
            public string EmpleadoNombre { get; set; } = null!;
            public bool EstaDisponible { get; set; }

            // Servicios que puede hacer ese barbero
            public List<ServicioItem> Servicios { get; set; } = new();
        }

        public class ServicioItem
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = null!;
            public decimal Precio { get; set; }
        }
    }
}
