using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Barberia.Models.Domain
{
        public class RegisterViewModel
        {
            public string Email { get; set; }
            public string Username { get; set; }

            public string Password { get; set; }

            public string ConfirmPassword { get; set; }
        }
    }

