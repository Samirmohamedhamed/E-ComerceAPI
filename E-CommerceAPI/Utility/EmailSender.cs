using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace E_CommerceAPI.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("samirmohammedhamed@gmail.com", "bxpb usdd aerz gotl")
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
