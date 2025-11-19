using System.Security.Claims;
using Barberia.Data;
using Barberia.Models.Domain;
using Barberia.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Barberia.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class UsuariosController : Controller
    {
        private readonly BarberiaContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuariosController(BarberiaContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        private int GetCurrentUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : 0;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index(string? search, string? rol)
        {
            var currentUserId = GetCurrentUserId();
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.Search = search;
            ViewBag.Rol = rol;

            var query = _context.Usuarios
                .Include(u => u.Persona)
                .Where(u => !u.EstaEliminado);

            // Buscar por usuario, nombre o apellido
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.NombreUsuario.Contains(search) ||
                    (u.Persona != null && (
                        u.Persona.Nombre.Contains(search) ||
                        u.Persona.Apellido.Contains(search)
                    ))
                );
            }

            // Filtrar por rol
            if (!string.IsNullOrWhiteSpace(rol))
            {
                switch (rol)
                {
                    case "admin":
                        query = query.Where(u => u.EsAdmin);
                        break;

                    case "barbero":
                        // usando la propiedad EsBarbero de Persona
                        query = query.Where(u => u.Persona != null && u.Persona.EsBarbero);
                        break;

                    case "usuario":
                        // ni admin ni barbero
                        query = query.Where(u =>
                            !u.EsAdmin &&
                            (u.Persona == null || !u.Persona.EsBarbero));
                        break;
                }
            }

            var usuarios = await query.ToListAsync();
            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Persona)
                    .ThenInclude(p => p.Empleado)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View(new UsuarioAdminCreateViewModel());
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioAdminCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = new Usuario
            {
                NombreUsuario = model.NombreUsuario,
                EsAdmin = model.EsAdmin,
                CreatedAt = DateTime.UtcNow
            };

            usuario.Contrasena = _passwordHasher.HashPassword(usuario, model.Password);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var persona = new Persona
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                CorreoElectronico = model.CorreoElectronico,
                EsBarbero = model.EsBarbero,
                UsuarioId = usuario.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();

            if (model.EsBarbero)
            {
                var empleado = new Empleado
                {
                    PersonaId = persona.Id
                };
                _context.Empleados.Add(empleado);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Persona)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            var vm = new Models.ViewModels.UsuarioAdminEditViewModel
            {
                Id = usuario.Id,
                NombreUsuario = usuario.NombreUsuario,
                EsAdmin = usuario.EsAdmin,
                EstaBloqueado = usuario.EstaBloqueado,
                EstaEliminado = usuario.EstaEliminado,
                Nombre = usuario.Persona?.Nombre ?? "",
                Apellido = usuario.Persona?.Apellido ?? "",
                CorreoElectronico = usuario.Persona?.CorreoElectronico,
                EsBarbero = usuario.Persona?.EsBarbero ?? false
            };

            return View(vm);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioAdminEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _context.Usuarios
                .Include(u => u.Persona)
                .FirstOrDefaultAsync(u => u.Id == model.Id);

            if (usuario == null)
                return NotFound();

            usuario.NombreUsuario = model.NombreUsuario;
            usuario.EsAdmin = model.EsAdmin;
            usuario.EstaBloqueado = model.EstaBloqueado;
            usuario.EstaEliminado = model.EstaEliminado;

            if (usuario.Persona == null)
            {
                usuario.Persona = new Persona
                {
                    UsuarioId = usuario.Id,
                    CreatedAt = DateTime.UtcNow
                };
            }

            usuario.Persona.Nombre = model.Nombre;
            usuario.Persona.Apellido = model.Apellido;
            usuario.Persona.CorreoElectronico = model.CorreoElectronico;
            usuario.Persona.EsBarbero = model.EsBarbero;

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.PersonaId == usuario.Persona.Id);

            if (model.EsBarbero && empleado == null)
            {
                _context.Empleados.Add(new Empleado { PersonaId = usuario.Persona.Id });
            }
            else if (!model.EsBarbero && empleado != null)
            {
                _context.Empleados.Remove(empleado);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Usuarios/Bloquear/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bloquear(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();
            if (usuario.Id == currentUserId)
            {
                TempData["Error"] = "No podés bloquear tu propio usuario.";
                return RedirectToAction(nameof(Index));
            }

            if (usuario.EstaEliminado)
            {
                TempData["Error"] = "No se puede bloquear un usuario eliminado.";
                return RedirectToAction(nameof(Index));
            }

            usuario.EstaBloqueado = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Usuarios/Desbloquear/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desbloquear(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();
            if (usuario.Id == currentUserId)
            {
                TempData["Error"] = "No tiene sentido desbloquearse a sí mismo desde acá.";
                return RedirectToAction(nameof(Index));
            }

            if (usuario.EstaEliminado)
            {
                TempData["Error"] = "No se puede desbloquear un usuario eliminado.";
                return RedirectToAction(nameof(Index));
            }

            usuario.EstaBloqueado = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Usuarios/Eliminar/5  (borrado lógico)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();
            if (usuario.Id == currentUserId)
            {
                TempData["Error"] = "No podés eliminar tu propio usuario.";
                return RedirectToAction(nameof(Index));
            }

            usuario.EstaEliminado = true;
            usuario.EstaBloqueado = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
