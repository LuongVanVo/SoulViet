using SoulViet.Shared.Application.DTOs;

namespace SoulViet.Shared.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendETicketEmailAsync(string toEmail, string customerName, string orderId, List<TicketEmailInfo> tickets);
}