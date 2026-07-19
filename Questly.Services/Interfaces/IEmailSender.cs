using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body);
    }
}
