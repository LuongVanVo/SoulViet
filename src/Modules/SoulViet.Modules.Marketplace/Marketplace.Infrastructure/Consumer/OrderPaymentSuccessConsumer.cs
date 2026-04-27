using MassTransit;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Events;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Consumer;

public class OrderPaymentSuccessConsumer : IConsumer<OrderPaymentSuccessEvent>
{
    private readonly ITicketOrchestrator _ticketOrchestrator;
    private readonly IMasterOrderRepository _masterOrderRepository;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;
    public OrderPaymentSuccessConsumer(ITicketOrchestrator ticketOrchestrator, IMasterOrderRepository masterOrderRepository, IEmailService emailService, IUserRepository userRepository)
    {
        _ticketOrchestrator = ticketOrchestrator;
        _masterOrderRepository = masterOrderRepository;
        _emailService = emailService;
        _userRepository = userRepository;
    }
    
    public async Task Consume(ConsumeContext<OrderPaymentSuccessEvent> context)
    {
        var tickets =
            await _ticketOrchestrator.ProcessTicketsForOrderAsync(context.Message.MasterOrderId,
                context.CancellationToken);

        if (tickets.Any())
        {
            var order = await _masterOrderRepository.GetByIdAsync(context.Message.MasterOrderId,
                context.CancellationToken);
            if (order == null) return;

            var user = await _userRepository.GetUserByIdAsync(order.UserId);

            await _emailService.SendETicketEmailAsync(user!.Email, user!.FullName, order.Id.ToString(), tickets);
        }
    }
}