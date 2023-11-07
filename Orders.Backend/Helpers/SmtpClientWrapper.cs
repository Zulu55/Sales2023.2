using MailKit.Net.Smtp;
using MimeKit;
using System.Diagnostics.CodeAnalysis;

namespace Orders.Backend.Helpers
{
    [ExcludeFromCodeCoverage(Justification = "It is a wrapper used to test other classes. There is no way to prove it.")]
    public class SmtpClientWrapper : ISmtpClient
    {
        private readonly SmtpClient _smtpClient = new SmtpClient();

        public void Authenticate(string username, string password) => _smtpClient.Authenticate(username, password);

        public void Connect(string host, int port, bool useSsl) => _smtpClient.Connect(host, port, useSsl);

        public void Disconnect(bool quit) => _smtpClient.Disconnect(quit);

        public void Send(MimeMessage message) => _smtpClient.Send(message);
    }
}