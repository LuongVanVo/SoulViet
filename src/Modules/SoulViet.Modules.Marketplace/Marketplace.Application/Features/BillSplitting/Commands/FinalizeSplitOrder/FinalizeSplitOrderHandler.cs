using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Events;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;
using SoulViet.Modules.Marketplace.Marketplace.Presentation.Hubs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Commands.FinalizeSplitOrder;

public class FinalizeSplitOrderHandler : IRequestHandler<FinalizeSplitOrderCommand, bool>
{
    private readonly ISplitRoomRepository _splitRoomRepository;
    private readonly IMasterOrderRepository _masterOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<BillSplitHub, IBillSplitClient> _hubContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ITicketOrchestrator _ticketOrchestrator;
    public FinalizeSplitOrderHandler(ISplitRoomRepository splitRoomRepository, IMasterOrderRepository masterOrderRepository, IUnitOfWork unitOfWork, IHubContext<BillSplitHub, IBillSplitClient> hubContext, IPublishEndpoint publishEndpoint, ITicketOrchestrator ticketOrchestrator)
    {
        _splitRoomRepository = splitRoomRepository;
        _masterOrderRepository = masterOrderRepository;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
        _publishEndpoint = publishEndpoint;
        _ticketOrchestrator = ticketOrchestrator;
    }

    public async Task<bool> Handle(FinalizeSplitOrderCommand request, CancellationToken cancellationToken)
    {
        var room = await _splitRoomRepository.GetRoomAsync(request.RoomId, cancellationToken);
        if (room == null) return false;

        var masterOrder = await _masterOrderRepository.GetByIdWithDetailsAsync(room.MasterOrderId, cancellationToken);
        if (masterOrder == null) return false;

        masterOrder.PaymentStatus = PaymentStatus.Success;
        masterOrder.PaymentDate = DateTime.UtcNow;

        foreach (var vendorOrder in masterOrder.VendorOrders)
        {
            vendorOrder.Status = OrderStatus.Processing;
        }

        var splitNote = string.Join(" | ", room.Members.Values.Select(m => $"{m.FullName}: {m.AmountToPay:N0}"));
        masterOrder.SplitNote = splitNote;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _ticketOrchestrator.ProcessTicketsForOrderAsync(masterOrder.Id, cancellationToken);

        await _publishEndpoint.Publish(new OrderPaymentSuccessEvent()
        {
            MasterOrderId = masterOrder.Id,
        }, cancellationToken);

        await _splitRoomRepository.DeleteRoomAsync(room.RoomId, cancellationToken);

        await _hubContext.Clients.Group(request.RoomId).ReceiveNotification("Congratulations! The bill has been successfully split and the order is now being processed.");

        await _hubContext.Clients.Group(request.RoomId).ReceiveRoomUpdate(room);
        return true;
    }
}