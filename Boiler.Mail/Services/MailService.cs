using Boiler.Mail.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Boiler.Mail.Services
{
    class MailService
    {
        private readonly MailSettings m_Settings;

        public MailService(IOptions<MailSettings> settings)
        {
            m_Settings = settings.Value;
        }

        public void Send(string from, string to, string subject, string html)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(m_Settings.SmtpHost, m_Settings.SmtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(m_Settings.SmtpUser, m_Settings.SmtpPass);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
