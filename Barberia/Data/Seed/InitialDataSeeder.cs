using Barberia.Data;
using Barberia.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Barberia.Data.Seed
{
    public static class InitialDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BarberiaContext>();

            await context.Database.MigrateAsync();

            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

            // ==========================
            // SEED USUARIO TEST (Admin)
            // ==========================
            if (!context.Usuarios.Any(u => u.NombreUsuario == "test"))
            {
                var usuario = new Usuario
                {
                    NombreUsuario = "test",
                    EsAdmin = true,
                    CreatedAt = DateTime.UtcNow
                };

                usuario.Contrasena = hasher.HashPassword(usuario, "123456");

                context.Usuarios.Add(usuario);
                await context.SaveChangesAsync();

                var persona = new Persona
                {
                    Nombre = "Test",
                    Apellido = "User",
                    CorreoElectronico = "test@barberia.local",
                    EsBarbero = false,
                    UsuarioId = usuario.Id,
                    CreatedAt = DateTime.UtcNow
                };

                context.Personas.Add(persona);
            }

            // ==========================
            // SEED 3 EMPLEADOS
            // ==========================


            await context.SaveChangesAsync();

            if (!context.Turnos.Any())
            {
                var turnos = new List<Turno>();

                var inicio = new DateTime(2025, 11, 20);
                var fin = new DateTime(2025, 12, 20);
                var horas = new[] { 9, 11, 14, 16, 18, 20 };

                var empleadosIds = new[] { 2, 3, 4 };
                var empleadoIndex = 0;

                for (var fecha = inicio; fecha <= fin; fecha = fecha.AddDays(1))
                {
                    if (fecha.DayOfWeek == DayOfWeek.Sunday)
                        continue;

                    foreach (var h in horas)
                    {
                        turnos.Add(new Turno
                        {
                            Fecha = fecha,
                            Hora = new TimeSpan(h, 0, 0),
                            EmpleadoId = empleadosIds[empleadoIndex % empleadosIds.Length],
                            EstaDisponible = true
                        });

                        empleadoIndex++;
                    }
                }

                context.Turnos.AddRange(turnos);
                await context.SaveChangesAsync();
            }
        }

    }
}
