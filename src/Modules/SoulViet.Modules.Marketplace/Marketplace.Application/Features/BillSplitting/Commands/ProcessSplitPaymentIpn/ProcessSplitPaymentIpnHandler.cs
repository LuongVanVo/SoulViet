using MediatR;
using Microsoft.AspNetCore.SignalR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Commands.FinalizeSplitOrder;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Presentation.Hubs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Commands.ProcessSplitPaymentIpn;

public class ProcessSplitPaymentIpnHandler : IRequestHandler<ProcessSplitPaymentIpnCommand, bool>
{
    private readonly ISplitRoomRepository _splitRoomRepository;
    private readonly IHubContext<BillSplitHub, IBillSplitClient> _hubContext;
    private readonly IMediator _mediator;
    public ProcessSplitPaymentIpnHandler(ISplitRoomRepository splitRoomRepository, IHubContext<BillSplitHub, IBillSplitClient> hubContext, IMediator mediator)
    {
        _splitRoomRepository = splitRoomRepository;
        _hubContext = hubContext;
        _mediator = mediator;
    }

    public async Task<bool> Handle(ProcessSplitPaymentIpnCommand request, CancellationToken cancellationToken)
    {
        if (request.ResponseCode != "00") return true;

        var parts = request.TxnRef.Split('-', 3);
        if (parts.Length < 3 || parts[0] != "SPLIT") return false;

        string roomId = parts[1];
        Guid userId;
        if (!Guid.TryParse(parts[2], out userId)) return false;

        var room = await _splitRoomRepository.GetRoomAsync(roomId, cancellationToken);
        if (room == null) return true;

        if (room.Members.TryGetValue(userId, out var member))
        {
            if (member.HasPaid) return true;

            member.HasPaid = true;
            await _splitRoomRepository.SetRoomAsync(room, cancellationToken: cancellationToken);

            await _hubContext.Clients.Group(roomId).ReceiveNotification($"{member.FullName} has completed their payment.");
            await _hubContext.Clients.Group(roomId).ReceiveMemberPaidSuccess(userId);
            await _hubContext.Clients.Group(roomId).ReceiveRoomUpdate(room);

            if (room.Members.Values.All(m => m.HasPaid))
            {
                var finalizeCommand = new FinalizeSplitOrderCommand { RoomId = roomId };
                await _mediator.Send(finalizeCommand, cancellationToken);
            }
        }

        return true;
    }
}