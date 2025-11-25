namespace Barberia.Interfaces
{
    public interface IAppEmailSender
    {
        Task SendAsync(string to, string subject, string htmlBody);
    }
}
