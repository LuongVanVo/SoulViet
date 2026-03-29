using MassTransit;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Shared.Infrastructure.Consumer;

public class ForgotPasswordConsumer : IConsumer<ForgotPasswordEvent>
{
    private readonly IEmailService _emailService;
    public ForgotPasswordConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<ForgotPasswordEvent> context)
    {
        var message = context.Message;

        string subject;
        string body;

        if (message.Language?.ToLower() == "vi")
        {
            subject = "Yêu cầu khôi phục mật khẩu";
            body = $@"
                <h2>Yêu cầu đặt lại mật khẩu</h2>
                <p>Bấm vào nút dưới đây để đặt lại mật khẩu mới:</p>
                <a href='{message.ResetLink}' style='background: blue; color: white; padding: 10px;text-decoration:none;'>Đặt lại mật khẩu</a>
                <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>";
        }
        else
        {
            subject = "Password Reset Request";
            body = $@"
                <h2>Password Reset Request</h2>
                <p>Click the button below to reset your password:</p>
                <a href='{message.ResetLink}' style='background: blue; color: white; padding: 10px;text-decoration:none;'>Reset Password</a>
                <p>If you did not request this, please ignore this email.</p>";
        }

        await _emailService.SendEmailAsync(message.Email, subject, body);
    }
}