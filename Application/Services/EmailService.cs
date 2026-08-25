using ApplicationServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Resend;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly IConfiguration _configuration;

    public EmailService(
        IResend resend,
        IConfiguration configuration)
    {
        _resend = resend;
        _configuration = configuration;
    }

    public async Task SendVerificationCodeAsync(
        string email,
        string code)
    {
        var fromEmail =
            _configuration["Resend:FromEmail"];

        var message = new EmailMessage
        {
            From = fromEmail!,
            Subject = "Ticketing System - Email Verification",
            HtmlBody = $@"
                <h2>Email Verification</h2>

                <p>Your verification code is:</p>

                <h1>{code}</h1>

                <p>This code will expire in 10 minutes.</p>

                <p>If you did not create this account,
                please ignore this email.</p>
            "
        };

        message.To.Add(email);

        await _resend.EmailSendAsync(message);
    }
}