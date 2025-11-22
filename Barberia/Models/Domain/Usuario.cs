using System.ComponentModel.DataAnnotations;

namespace Barberia.Models.Domain
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreUsuario { get; set; } = null!;

        [Required]
        public string Contrasena { get; set; } = null!;

        public bool EsAdmin { get; set; }
        public bool EstaBloqueado { get; set; }
        public bool EstaEliminado { get; set; }

        public DateTime CreatedAt { get; set; }

        // 🔹 para recuperación de contraseña
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiresAt { get; set; }

        // Navegación
        public Persona? Persona { get; set; }
        public List<Reserva> Reservas { get; set; } = new();
    }
}
