using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tryout.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string subject, string email, string htmlMessage)
        {
            //Logic to send email
            return Task.CompletedTask;
        }
    }
}
