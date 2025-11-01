using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Resend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tryout.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IResend _resendClient;
        public EmailSender(IOptions<ResendSetting> options)
        {
            var apiKey = options.Value.ApiKey;
            _resendClient = ResendClient.Create(apiKey);
        }
        public async Task SendEmailAsync(string subject, string email, string htmlMessage)
        {
            email = email?.Trim();
            Console.WriteLine($"📤 EmailSender called. To: {email ?? "null"} | Subject: {subject}");
            //Logic to send email
            var message = new EmailMessage()
            {
                From = "Kamshi Store <no-reply@kamshi.store>",
                To =  email,
                Subject = subject,
                HtmlBody = htmlMessage
            };

            await _resendClient.EmailSendAsync(message);
        }
    }
}
