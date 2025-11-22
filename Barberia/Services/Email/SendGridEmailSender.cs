using Barberia.Interfaces;
using Microsoft.Extensions.Logging;   // <-- IMPORTANTE
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Barberia.Services.Email
{
    public class SendGridEmailSender : IAppEmailSender
    {
        private readonly SendGridSettings _settings;
        private readonly ILogger<SendGridEmailSender> _logger;   // <-- logger

        public SendGridEmailSender(
            IOptions<SendGridSettings> options,
            ILogger<SendGridEmailSender> logger)   // <-- logger inyectado
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var client = new SendGridClient(_settings.ApiKey);

            var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
            var toAddress = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(
                from,
                toAddress,
                subject,
                plainTextContent: null,
                htmlContent: htmlBody
            );

            _logger.LogInformation("📨 Enviando email a {Email} con asunto '{Subject}'...", to, subject);

            var response = await client.SendEmailAsync(msg);

            _logger.LogInformation("📬 Código de respuesta SendGrid: {StatusCode}", response.StatusCode);

            var responseBody = await response.Body.ReadAsStringAsync();
            _logger.LogInformation("📄 Respuesta SendGrid: {Body}", responseBody);
        }
    }
}
