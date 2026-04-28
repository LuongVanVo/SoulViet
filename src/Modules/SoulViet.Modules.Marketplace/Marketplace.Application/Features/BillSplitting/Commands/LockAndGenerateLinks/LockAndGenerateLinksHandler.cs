using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Commands.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Presentation.Hubs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Commands.LockAndGenerateLinks;

public class LockAndGenerateLinksHandler : IRequestHandler<LockAndGenerateLinksCommand, LockAndGenerateLinksResponse>
{
    private readonly ISplitRoomRepository _splitRoomRepository;
    private readonly IVnPayService _vnPayService;
    private readonly IHubContext<BillSplitHub, IBillSplitClient> _hubContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public LockAndGenerateLinksHandler(ISplitRoomRepository splitRoomRepository, IVnPayService vnPayService, IHubContext<BillSplitHub, IBillSplitClient> hubContext, IHttpContextAccessor httpContextAccessor)
    {
        _splitRoomRepository = splitRoomRepository;
        _vnPayService = vnPayService;
        _hubContext = hubContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LockAndGenerateLinksResponse> Handle(LockAndGenerateLinksCommand request, CancellationToken cancellationToken)
    {
        var room = await _splitRoomRepository.GetRoomAsync(request.RoomId, cancellationToken);
        if (room == null)
            throw new NotFoundException("Split room not found or has expired.");

        if (room.HostUserId != request.HostUserId)
            throw new UnauthorizedAccessException("Only the host can lock the room and generate payment links.");

        if (!room.IsAmountValid())
            throw new BadRequestException("Total amount does not match the sum of individual shares. Please adjust the amounts before locking.");

        if (room.Members.Values.Any(m => !m.IsReady))
            throw new BadRequestException("Some members are not ready yet. Please wait until all members are ready before locking the room.");

        if (room.IsLocked)
            throw new BadRequestException("The room is already locked.");

        room.IsLocked = true;

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            throw new InvalidOperationException("Unable to access HTTP context.");

        // Generate link payment for each member
        foreach (var member in room.Members.Values)
        {
            if (member.AmountToPay > 0)
            {
                string txnRef = $"SPLIT-{room.RoomId}-{member.UserId}";
                string orderInfo = $"Payment for split bill in room {room.RoomId} - User {member.FullName}";

                string url = _vnPayService.CreatePaymentUrl(member.AmountToPay, txnRef, orderInfo, httpContext);

                member.PaymentUrl = url;

                if (!string.IsNullOrEmpty(member.ConnectionId))
                {
                    await _hubContext.Clients.Client(member.ConnectionId).ReceivePaymentUrl(url);
                }
            }
            else
            {
                member.HasPaid = true;
            }
        }

        await _splitRoomRepository.SetRoomAsync(room, cancellationToken: cancellationToken);

        await _hubContext.Clients.Group(room.RoomId).ReceiveRoomUpdate(room);

        return new LockAndGenerateLinksResponse
        {
            Success = true,
            Message = "Room locked and payment links generated successfully.",
        };
    }
}