using Microsoft.Extensions.Logging;
using MimeKit;
using SoulViet.Shared.Application.Common.ExternalSettings;
using SoulViet.Shared.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace SoulViet.Shared.Infrastructure.Services;

public class EmailService(MailSettings mailSettings) : IEmailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        // Create content for the email
        var email = new MimeMessage();

        // From
        email.From.Add(new MailboxAddress(mailSettings.SenderName, mailSettings.SenderEmail));

        // To
        email.To.Add(MailboxAddress.Parse(toEmail));

        // Subject
        email.Subject = subject;

        // Body
        var builder = new BodyBuilder();
        builder.HtmlBody = body;
        email.Body = builder.ToMessageBody();

        // Connect to the SMTP server and send the email
        using var smtp = new SmtpClient();
        try
        {
            // Connect to the SMTP server via PORT 587 with STARTTLS
            await smtp.ConnectAsync(mailSettings.Server, mailSettings.Port, SecureSocketOptions.StartTls);
            // Authenticate with the SMTP server
            await smtp.AuthenticateAsync(mailSettings.SenderEmail, mailSettings.Password);
            // Send the email
            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
            throw;
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}