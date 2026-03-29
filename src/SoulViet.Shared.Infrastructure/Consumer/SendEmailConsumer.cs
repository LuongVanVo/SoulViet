using MassTransit;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Shared.Infrastructure.Consumer;

public class SendEmailConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IEmailService _emailService;
    public SendEmailConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var message = context.Message;
        await _emailService.SendEmailAsync(message.Email, message.Subject, message.Body);
    }
}