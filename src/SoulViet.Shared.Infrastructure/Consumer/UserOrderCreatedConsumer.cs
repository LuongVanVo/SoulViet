using MassTransit;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Shared.Infrastructure.Consumer;
public class UserOrderCreatedConsumer : IConsumer<UserOrderCreatedEvent>
{
    private readonly IEmailService _emailService;
    public UserOrderCreatedConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task Consume(ConsumeContext<UserOrderCreatedEvent> context)
    {
        var message = context.Message;
        var isVietnamese = message.Language?.ToLower() == "vi";

        var subject = isVietnamese
            ? $"[SoulViet] Xác nhận đơn hàng #{message.MasterOrderId.ToString().Substring(0, 8).ToUpper()}"
            : $"[SoulViet] Order Confirmation #{message.MasterOrderId.ToString().Substring(0, 8).ToUpper()}";

        var css = @"
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
        .email-wrapper { width: 100%; padding: 40px 0; }
        .container { max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
        .header { background-color: #1a73e8; color: #ffffff; padding: 25px 20px; text-align: center; }
        .header h2 { margin: 0; font-size: 24px; }
        .body-content { padding: 30px; color: #333333; line-height: 1.6; }
        .order-box { background-color: #f8f9fa; border-left: 4px solid #1a73e8; padding: 15px; margin: 20px 0; border-radius: 4px; }
        .total-price { color: #d93025; font-size: 20px; font-weight: bold; }
        .btn-action { display: inline-block; background-color: #1a73e8; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 5px; font-weight: bold; margin-top: 20px; text-align: center; }
        .footer { background-color: #eeeeee; text-align: center; padding: 20px; font-size: 13px; color: #777777; }
    ";

        var orderLink = Environment.GetEnvironmentVariable("CLIENT_URL") + $"/orders/{message.MasterOrderId}";
        var bodyContent = isVietnamese
            ? $@"
            <div class='header'><h2>Cảm ơn bạn đã đặt hàng!</h2></div>
            <div class='body-content'>
                <p>Chào <strong>{message.ReceiverName}</strong>,</p>
                <p>Đơn hàng của bạn đã được ghi nhận trên hệ thống SoulViet. Các Đối tác địa phương của chúng tôi đang chuẩn bị dịch vụ/sản phẩm cho bạn.</p>
                <div class='order-box'>
                    <p style='margin-top:0'><strong>Mã đơn hàng:</strong> {message.MasterOrderId}</p>
                    <p style='margin-bottom:0'><strong>Tổng thanh toán:</strong> <span class='total-price'>{message.GrandTotal:N0} VNĐ</span></p>
                </div>
                <p>Để theo dõi chi tiết hoặc yêu cầu hỗ trợ, vui lòng click vào nút bên dưới:</p>
                <div style='text-align: center;'>
                    <a href='{orderLink}' class='btn-action'>Xem Chi Tiết Đơn Hàng</a>
                </div>
            </div>"
            : $@"
            <div class='header'><h2>Thank you for your order!</h2></div>
            <div class='body-content'>
                <p>Hi <strong>{message.ReceiverName}</strong>,</p>
                <p>Your order has been successfully placed on SoulViet. Our Local Partners are now preparing your service/product.</p>
                <div class='order-box'>
                    <p style='margin-top:0'><strong>Order ID:</strong> {message.MasterOrderId}</p>
                    <p style='margin-bottom:0'><strong>Total Amount:</strong> <span class='total-price'>{message.GrandTotal:N0} VND</span></p>
                </div>
                <p>To track your order details or request support, please click the button below:</p>
                <div style='text-align: center;'>
                    <a href='{orderLink}' class='btn-action'>View Order Details</a>
                </div>
            </div>";

        var body = $"<html><head><style>{css}</style></head><body><div class='email-wrapper'><div class='container'>{bodyContent}<div class='footer'>© {DateTime.Now.Year} SoulViet. All rights reserved.</div></div></div></body></html>";

        return _emailService.SendEmailAsync(message.ReceiverEmail, subject, body);
    }
}
