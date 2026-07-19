using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Questly.Services.Configurations;
using Questly.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace Questly.Services.Implementations
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var client = new SmtpClient();
            var message = new MimeMessage();

            message.From.Add(MailboxAddress.Parse(_settings.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            await client.ConnectAsync(_settings.Host, _settings.Port, false);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
