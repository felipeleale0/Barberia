using System.ComponentModel.DataAnnotations;

namespace Barberia.Models.Domain
{
    public class Persona
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Apellido { get; set; } = null!;

        public string? CorreoElectronico { get; set; }

        public bool EsBarbero { get; set; }
        public DateTime CreatedAt { get; set; }  

        [Required]
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; } = null!;

        // Si la persona es barbero, tendrá un Empleado asociado
        public Empleado? Empleado { get; set; }
    }
}
