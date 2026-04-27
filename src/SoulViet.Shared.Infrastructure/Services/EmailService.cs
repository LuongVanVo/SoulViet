using System.Text;
using MimeKit;
using SoulViet.Shared.Application.Common.ExternalSettings;
using SoulViet.Shared.Application.Interfaces;
using MailKit.Security;
using SoulViet.Shared.Application.DTOs;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

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

    public async Task SendETicketEmailAsync(string toEmail, string customerName, string orderId, List<TicketEmailInfo> tickets)
    {
        if (string.IsNullOrEmpty(toEmail) || !tickets.Any())
            return;

        var htmlBody = BuildETicketHtmlTemplate(customerName, orderId, tickets);

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(mailSettings.SenderName, mailSettings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = $"[SoulViet] Vé điện tử cho đơn hàng #{orderId}";

        var builder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };
        email.Body = builder.ToMessageBody();

        using var smtpClient = new SmtpClient();

        try
        {
            await smtpClient.ConnectAsync(mailSettings.Server, mailSettings.Port, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(mailSettings.SenderEmail, mailSettings.Password);
            await smtpClient.SendAsync(email);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to send e-ticket email: " + ex.Message);
        }
        finally
        {
            await smtpClient.DisconnectAsync(true);
        }
    }

    private string BuildETicketHtmlTemplate(string customerName, string orderId, List<TicketEmailInfo> tickets)
    {
        var sb = new StringBuilder();

        // Header
        sb.Append($@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px;'>
                <h2 style='color: #007bff; text-align: center;'>VÉ ĐIỆN TỬ SOULVIET</h2>
                <p>Chào <strong>{customerName}</strong>,</p>
                <p>Cảm ơn bạn đã đặt dịch vụ tại SoulViet. Đây là vé điện tử cho đơn hàng <strong>#{orderId}</strong> của bạn.</p>
                <hr/>");

        // Vòng lặp in ra từng vé
        foreach (var ticket in tickets)
        {
            sb.Append($@"
                <div style='margin-bottom: 30px; padding: 15px; background: #f9f9f9; border-radius: 8px;'>
                    <h3 style='margin-top: 0;'>{ticket.ProductName}</h3>
                    <p>Số lượng: {ticket.Quantity}</p>
                    <div style='text-align: center;'>
                        <img src='{ticket.QrUrl}' alt='QR Code' style='width: 200px; height: 200px; border: 1px solid #eee; display: inline-block;'/>
                        <p style='font-weight: bold; font-size: 18px; letter-spacing: 2px; margin-top: 10px;'>{ticket.TicketCode}</p>
                    </div>
                    <p style='font-size: 12px; color: #666; text-align: center;'>(Vui lòng đưa mã này cho nhân viên tại địa điểm để check-in)</p>
                </div>");
        }

        // Footer
        sb.Append(@"
                <div style='margin-top: 20px; font-size: 14px; color: #555; text-align: center;'>
                    <p>Chúc bạn có một chuyến đi trải nghiệm tuyệt vời!</p>
                    <p><strong>Đội ngũ SoulViet.</strong></p>
                </div>
            </div>");

        return sb.ToString();
    }
}
