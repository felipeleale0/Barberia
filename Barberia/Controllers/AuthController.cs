using Barberia.Data;
using Barberia.Interfaces;
using Barberia.Models.Domain;
using Barberia.Models.ViewModels;
using Barberia.Services.Email;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Barberia.Controllers
{
    public class AuthController : Controller
    {
        private readonly BarberiaContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly IAppEmailSender _emailSender;   // ✅
        private readonly EmailTemplateLoader _templateLoader;

        public AuthController(BarberiaContext context, IPasswordHasher<Usuario> passwordHasher, IAppEmailSender emailSender, EmailTemplateLoader templateLoader)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _emailSender = emailSender;
            _templateLoader = templateLoader;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            // si ya está logueado, no tiene sentido mostrar el login
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Usuarios
                .Include(u => u.Persona)
                .FirstOrDefaultAsync(u => u.NombreUsuario == model.UserName);

            if (user == null || user.EstaEliminado)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View(model);
            }

            if (user.EstaBloqueado)
            {
                ModelState.AddModelError(string.Empty, "El usuario está bloqueado. Contactá con administración.");
                return View(model);
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Contrasena, model.Password);

            if (result != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View(model);
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.NombreUsuario),
            new Claim("EsAdmin", user.EsAdmin.ToString())
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize] // sólo alguien logueado puede hacer logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            // etc...
            return View();
        }

        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Buscar usuario por mail de Persona
            var user = await _context.Usuarios
                .Include(u => u.Persona)
                .FirstOrDefaultAsync(u => u.Persona != null
                                          && u.Persona.CorreoElectronico == model.Email
                                          && !u.EstaEliminado);

            // Siempre devolvemos el mismo mensaje, exista o no el mail (por seguridad)
            if (user == null)
            {
                TempData["ForgotPasswordMessage"] =
                    "Si el correo existe en el sistema, te enviamos un mail con instrucciones.";
                return RedirectToAction("ForgotPassword");
            }

            // Generar token aleatorio
            var tokenBytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(tokenBytes);

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1); // p.ej. 1 hora
            await _context.SaveChangesAsync();

            // URL de reseteo
            var callbackUrl = Url.Action(
                "ResetPassword",
                "Auth",
                new { userId = user.Id, token = token },
                protocol: Request.Scheme);

            // 🔹 Cargar template HTML y reemplazar placeholders
            var template = _templateLoader.LoadTemplate("ResetPassword.html");

            template = template.Replace("{{Nombre}}", user.Persona?.Nombre ?? user.NombreUsuario);
            template = template.Replace("{{Link}}", callbackUrl);


            // Enviar mail usando el template
            await _emailSender.SendAsync(
                model.Email,
                "Recuperar contraseña - Barbería",
                template);

            TempData["ForgotPasswordMessage"] =
                "Si el correo existe en el sistema, te enviamos un mail con instrucciones.";
            return RedirectToAction("ForgotPassword");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(int userId, string token)
        {
            if (userId <= 0 || string.IsNullOrEmpty(token))
            {
                return BadRequest("Enlace inválido.");
            }

            // Armamos el modelo para la vista
            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == model.UserId && !u.EstaEliminado);

            if (user == null)
                return NotFound();

            // Validar token
            if (string.IsNullOrEmpty(user.PasswordResetToken) ||
                user.PasswordResetToken != model.Token ||
                user.PasswordResetTokenExpiresAt == null ||
                user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
            {
                ModelState.AddModelError(string.Empty,
                    "El enlace para restablecer la contraseña no es válido o ya expiró.");
                return View(model);
            }

            // Actualizar contraseña
            user.Contrasena = _passwordHasher.HashPassword(user, model.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiresAt = null;

            await _context.SaveChangesAsync();

            TempData["LoginMessage"] = "Tu contraseña fue actualizada correctamente. Ingresá con tu nueva contraseña.";
            return RedirectToAction("Login");
        }
    }
}
