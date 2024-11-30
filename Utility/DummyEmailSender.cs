using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;


namespace Utility
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // تنفيذ فارغ لا يفعل شيئًا
            return Task.CompletedTask;
        }
    }

}

