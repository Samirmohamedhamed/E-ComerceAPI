using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace E_CommerceAPI.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // The Gmail App Password is read from an environment variable (never hard-code secrets).
            var appPassword = Environment.GetEnvironmentVariable("EMAIL_APP_PASSWORD");

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("samirmohammedhamed@gmail.com", appPassword)
            };
            return client.SendMailAsync(
                new MailMessage(
                    from: "Your.email@live.com",
                    to: email,
                    subject, htmlMessage)
                {
                    IsBodyHtml = true
                });
        }
    }
}
