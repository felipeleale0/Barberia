using Microsoft.AspNetCore.Hosting;

namespace Barberia.Services.Email
{
    public class EmailTemplateLoader
    {
        private readonly IWebHostEnvironment _env;

        public EmailTemplateLoader(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string LoadTemplate(string fileName)
        {
            // wwwroot/template/ResetPassword.html
            var path = Path.Combine(_env.WebRootPath, "template", fileName);

            Console.WriteLine("[EmailTemplateLoader] path = " + path);
            Console.WriteLine("[EmailTemplateLoader] existe carpeta? " +
                              Directory.Exists(Path.Combine(_env.WebRootPath, "template")));
            Console.WriteLine("[EmailTemplateLoader] existe archivo? " + File.Exists(path));

            if (!File.Exists(path))
                throw new FileNotFoundException($"No se encontró el template en {path}");

            return File.ReadAllText(path);
        }
    }
}
