using Microsoft.AspNetCore.Http;
using QRCoder;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly ICloudflareR2Service _cloudflareR2Service;
    public TicketService(ICloudflareR2Service cloudflareR2Service)
    {
        _cloudflareR2Service = cloudflareR2Service;
    }

    public async Task<string> GenerateAndUploadQrCodeAsync(string ticketCode)
    {
        // Generate QR Code using a library like QRCoder
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(ticketCode, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrBytes = qrCode.GetGraphic(20);

        // Convert byte array to IFormFile for uploading
        using var stream = new MemoryStream(qrBytes);
        var formFile = new FormFile(stream, 0, qrBytes.Length, "file", $"ticket_{Guid.NewGuid()}.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        var uploadResult = await _cloudflareR2Service.UploadImageAsync(formFile, "e-tickets");

        return uploadResult.Url;
    }
}