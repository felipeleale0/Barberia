using Barberia.Models;
using Microsoft.AspNetCore.Mvc;

namespace Barberia.Models
{
    public class BarberoServicio
    {
        public int BarberoId { get; set; }
        public Barbero Barbero { get; set; }

        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }
    }
}