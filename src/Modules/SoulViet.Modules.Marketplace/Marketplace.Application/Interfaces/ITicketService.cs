namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;

public interface ITicketService
{
    Task<string> GenerateAndUploadQrCodeAsync(string ticketCode);
}