using Microsoft.AspNetCore.Mvc;

namespace Barberia.Models.Domain
{
    public class BarberoServicio
    {
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; } = null!;

        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; } = null!;
    }
}