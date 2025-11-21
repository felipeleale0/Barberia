using System.ComponentModel.DataAnnotations;

namespace Barberia.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = null!;
    }
}
