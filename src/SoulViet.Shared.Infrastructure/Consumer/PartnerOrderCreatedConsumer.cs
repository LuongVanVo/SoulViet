using MassTransit;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Shared.Infrastructure.Consumer;

public class PartnerOrderCreatedConsumer : IConsumer<PartnerOrderCreatedEvent>
{
    private readonly IEmailService _emailService;
    public PartnerOrderCreatedConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<PartnerOrderCreatedEvent> context)
    {
        var message = context.Message;
        var isVietnamese = message.Language == "vi";

        var subject = isVietnamese
            ? $"[SoulViet Kênh Người Bán] Bạn có 1 đơn hàng mới!"
            : $"[SoulViet Seller] You have a new order!";

        var css = @"
            body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
            .email-wrapper { width: 100%; padding: 40px 0; }
            .container { max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
            .header { background-color: #FF9800; color: #ffffff; padding: 25px 20px; text-align: center; } /* Màu cam cho kênh người bán */
            .header h2 { margin: 0; font-size: 24px; }
            .body-content { padding: 30px; color: #333333; line-height: 1.6; }
            .order-box { background-color: #fff8e1; border-left: 4px solid #FF9800; padding: 15px; margin: 20px 0; border-radius: 4px; }
            .total-price { color: #d93025; font-size: 20px; font-weight: bold; }
            .btn-action { display: inline-block; background-color: #FF9800; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 5px; font-weight: bold; margin-top: 20px; text-align: center; }
            .footer { background-color: #eeeeee; text-align: center; padding: 20px; font-size: 13px; color: #777777; }
        ";

        var adminLink = Environment.GetEnvironmentVariable("PARTNER_DASHBOARD_URL") + $"/orders/{message.OrderId}";

        var bodyContent = isVietnamese
            ? $@"
                <div class='header'><h2>Đơn hàng mới từ SoulViet</h2></div>
                <div class='body-content'>
                    <p>Chào <strong>Đối tác</strong>,</p>
                    <p>Gian hàng của bạn vừa nhận được một đơn hàng mới từ khách hàng <strong>{message.CustomerName}</strong>.</p>
                    <div class='order-box'>
                        <p style='margin-top:0'><strong>Mã đơn hàng shop:</strong> {message.OrderId}</p>
                        <p style='margin-bottom:0'><strong>Giá trị đơn hàng:</strong> <span class='total-price'>{message.TotalAmount:N0} VNĐ</span></p>
                    </div>
                    <p>Vui lòng đăng nhập vào trang Quản lý Đối tác (Partner Dashboard) để tiếp nhận và chuẩn bị đơn hàng sớm nhất nhé!</p>
                    <div style='text-align: center;'>
                        <a href='{adminLink}' class='btn-action'>Quản Lý Đơn Hàng</a>
                    </div>
                </div>"
            : $@"
                <div class='header'><h2>New Order Received</h2></div>
                <div class='body-content'>
                    <p>Hi <strong>Partner</strong>,</p>
                    <p>Your shop just received a new order from <strong>{message.CustomerName}</strong>.</p>
                    <div class='order-box'>
                        <p style='margin-top:0'><strong>Shop Order ID:</strong> {message.OrderId}</p>
                        <p style='margin-bottom:0'><strong>Order Value:</strong> <span class='total-price'>{message.TotalAmount:N0} VND</span></p>
                    </div>
                    <p>Please log in to your Partner Dashboard to accept and prepare this order as soon as possible!</p>
                    <div style='text-align: center;'>
                        <a href='{adminLink}' class='btn-action'>Manage Order</a>
                    </div>
                </div>";

        var body = $"<html><head><style>{css}</style></head><body><div class='email-wrapper'><div class='container'>{bodyContent}<div class='footer'>© {DateTime.Now.Year} SoulViet Seller Center.</div></div></div></body></html>";

        await _emailService.SendEmailAsync(message.PartnerEmail, subject, body);
    }
}