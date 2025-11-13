using Barberia.Data;
using Barberia.Models.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<BarberiaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionBarberia")));

// MVC
builder.Services.AddControllersWithViews();

// Hasher para Usuario (lo inyectamos en controllers y en el seed)
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

// Autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Si intenta acceder a algo con [Authorize] y no está autenticado
        options.LoginPath = "/Auth/Login";

        // (Opcional) si hace logout usando el esquema estándar
        options.LogoutPath = "/Auth/Logout";

        // Si no tiene permisos (p.ej. [Authorize(Policy = ...)])
        options.AccessDeniedPath = "/Auth/Login";

        // Opcional: duración de la cookie
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

// Autorización con políticas (incluimos AdminOnly basada en el claim EsAdmin)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("EsAdmin", "True"));
});

var app = builder.Build();

// SEED inicial (usuario test + persona)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BarberiaContext>();
    context.Database.Migrate();

    if (!context.Usuarios.Any(u => u.NombreUsuario == "test"))
    {
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

        var usuario = new Usuario
        {
            NombreUsuario = "test",
            EsAdmin = true,
            CreatedAt = DateTime.UtcNow
        };

        usuario.Contrasena = hasher.HashPassword(usuario, "123456");

        context.Usuarios.Add(usuario);
        context.SaveChanges();

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
        context.SaveChanges();
    }
}

// PIPELINE HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Ruta por defecto: Home/Index (con [Authorize])
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
