using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace TMS.Infrastructure.Helpers
{
    public class EmailHelper
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailHelper(IConfiguration configuration)
        {
            _host = configuration["EmailSettings:Host"]!;
            _port = int.Parse(configuration["EmailSettings:Port"] ?? "587");
            _username = configuration["EmailSettings:Username"]!;
            _password = configuration["EmailSettings:Password"]!;
            _fromEmail = configuration["EmailSettings:FromEmail"]!;
            _fromName = configuration["EmailSettings:FromName"] ?? "TMS System";
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);

            if (!string.IsNullOrEmpty(_username))
                await client.AuthenticateAsync(_username, _password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
