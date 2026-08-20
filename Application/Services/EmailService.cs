using ApplicationServices.Interfaces;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _email;
        private readonly string _password;

        public EmailService(
            IConfiguration configuration)
        {
            _email = configuration["EmailSettings:Email"]
                ?? throw new InvalidOperationException(
                    "Email is not configured.");

            _password = configuration["EmailSettings:Password"]
                ?? throw new InvalidOperationException(
                    "Email password is not configured.");
        }

        public async Task SendVerificationCodeAsync(
            string email,
            string code)
        {
            using var message = new MailMessage();

            message.From = new MailAddress(_email);
            message.To.Add(email);

            message.Subject = "Email Verification Code";

            message.Body =
                $"Your verification code is: {code}\n\n" +
                "This code will expire in 10 minutes.";

            message.IsBodyHtml = false;

            using var smtp = new SmtpClient("smtp.gmail.com", 587);

            smtp.EnableSsl = true;

            smtp.Credentials = new NetworkCredential(
                _email,
                _password);

            await smtp.SendMailAsync(message);
        }
    }
}