using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Barberia.Data
{
    public class BarberiaContextFactory : IDesignTimeDbContextFactory<BarberiaContext>
    {
        public BarberiaContext CreateDbContext(string[] args)
        {
            // Obtener el path base del proyecto
            var basePath = Directory.GetCurrentDirectory();

            // Cargar configuración desde appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            // Leer cadena de conexión
            var connectionString = config.GetConnectionString("ConexionBarberia");

            // Configurar opciones del contexto
            var optionsBuilder = new DbContextOptionsBuilder<BarberiaContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new BarberiaContext(optionsBuilder.Options);
        }
    }
}
