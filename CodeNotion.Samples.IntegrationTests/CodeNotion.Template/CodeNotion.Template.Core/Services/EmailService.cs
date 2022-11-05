using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CodeNotion.Template.Business.Configurators;

namespace CodeNotion.Template.Business.Services
{
    public interface IEmailService
    {
        Task SendEmailMessageAsync(MailMessage message);
    }

    public class EmailService : IEmailService
    {
        protected readonly SmtpClient SmtpClient;
        protected readonly ICoreConfiguration Config;

        public EmailService(SmtpClient smtpClient, ICoreConfiguration config)
        {
            SmtpClient = smtpClient;
            Config = config;
        }

        public async Task SendEmailMessageAsync(MailMessage message)
        {
            try
            {
                SmtpClient.Host = Config.EmailConfigHost;
                SmtpClient.Port = Config.EmailConfigPort;
                SmtpClient.EnableSsl = true;
                SmtpClient.Credentials = new NetworkCredential(Config.EmailConfigSender, Config.EmailConfigPassword);
                await SmtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send order email: {ex}");
            }
        }
    }
}