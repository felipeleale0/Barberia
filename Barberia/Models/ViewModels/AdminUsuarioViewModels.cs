using System.ComponentModel.DataAnnotations;

namespace Barberia.Models.ViewModels
{
    public class UsuarioAdminCreateViewModel
    {
        // Usuario
        [Required]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; } = null!;

        [Display(Name = "Es administrador")]
        public bool EsAdmin { get; set; }

        // Persona
        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Apellido { get; set; } = null!;

        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string? CorreoElectronico { get; set; }

        [Display(Name = "Es barbero / empleado")]
        public bool EsBarbero { get; set; }
    }

    public class UsuarioAdminEditViewModel
    {
        public int Id { get; set; }

        // Usuario
        [Required]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = null!;

        [Display(Name = "Es administrador")]
        public bool EsAdmin { get; set; }

        [Display(Name = "Bloqueado")]
        public bool EstaBloqueado { get; set; }

        [Display(Name = "Eliminado (lógico)")]
        public bool EstaEliminado { get; set; }

        // Persona
        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Apellido { get; set; } = null!;

        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string? CorreoElectronico { get; set; }

        [Display(Name = "Es barbero / empleado")]
        public bool EsBarbero { get; set; }
    }
}
