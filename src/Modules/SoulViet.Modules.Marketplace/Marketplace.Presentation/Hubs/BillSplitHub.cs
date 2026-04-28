using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Presentation.Hubs;

[Authorize]
public class BillSplitHub : Hub<IBillSplitClient>
{
    private readonly ISplitRoomRepository _splitRoomRepository;
    public BillSplitHub(ISplitRoomRepository splitRoomRepository)
    {
        _splitRoomRepository = splitRoomRepository;
    }

    private Guid GetUserId() => Guid.Parse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
    private string GetFullName() => Context.User?.FindFirst("full_name")?.Value ?? "Anonymous";

    // Create a new split room (Leader room creates)
    public async Task CreateRoom(Guid masterOrderId, decimal totalAmount)
    {
        var hostId = GetUserId();

        // Generate a unique room ID
        var roomId = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

        var room = new SplitRoomSession
        {
            RoomId = roomId,
            MasterOrderId = masterOrderId,
            HostUserId = hostId,
            TotalAmount = totalAmount,
            IsLocked = false,
        };

        // Add host as first member
        room.Members.Add(hostId, new SplitMemberState
        {
            UserId = hostId,
            FullName = GetFullName(),
            ConnectionId = Context.ConnectionId,
            AmountToPay = totalAmount,
            IsReady = false
        });

        await _splitRoomRepository.SetRoomAsync(room);

        // Add ConnectionId to group of SignalR
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        // Notify the creator about the room creation
        await Clients.Caller.ReceiveRoomUpdate(room);
    }

    // Join an existing split room
    public async Task JoinRoom(string roomId)
    {
        var room = await _splitRoomRepository.GetRoomAsync(roomId);
        if (room == null)
        {
            await Clients.Caller.ReceiveError("Split room not exists or has expired.");
            return;
        }

        if (room.IsLocked)
        {
            await Clients.Caller.ReceiveError("Split room is locked. No more members can join.");
            return;
        }

        var userId = GetUserId();

        // If user not exist in room, add to room
        if (!room.Members.ContainsKey(userId))
        {
            room.Members.Add(userId, new SplitMemberState
            {
                UserId = userId,
                FullName = GetFullName(),
                ConnectionId = Context.ConnectionId,
                AmountToPay = 0, // Mặc định là 0, leader sẽ phân bổ lại sau
            });

            await _splitRoomRepository.SetRoomAsync(room);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            // Notify all members in room about the update
            await Clients.Group(roomId).ReceiveNotification($"{GetFullName()} has joined the split room.");
            await Clients.Group(roomId).ReceiveRoomUpdate(room);
        }
        else
        {
            room.Members[userId].ConnectionId = Context.ConnectionId; // Cập nhật lại ConnectionId nếu người dùng đã tồn tại (trường hợp bị mất kết nối rồi vào lại)
            await _splitRoomRepository.SetRoomAsync(room);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).ReceiveRoomUpdate(room);
        }
    }

    // Update amount
    public async Task UpdateAmount(string roomId, decimal newAmount)
    {
        var room = await _splitRoomRepository.GetRoomAsync(roomId);
        if (room == null || room.IsLocked) return;

        var userId = GetUserId();
        if (room.Members.TryGetValue(userId, out var member))
        {
            member.AmountToPay = newAmount;

            // Cancel status Ready of All member because the amount has changed, they need to re-confirm
            foreach (var m in room.Members.Values) m.IsReady = false;

            await _splitRoomRepository.SetRoomAsync(room);

            await Clients.Group(roomId).ReceiveRoomUpdate(room);
        }
    }

    /// 4. XÁC NHẬN SẴN SÀNG (Bấm nút Ready)
    public async Task ToggleReady(string roomId)
    {
        var room = await _splitRoomRepository.GetRoomAsync(roomId);
        if (room == null || room.IsLocked) return;

        var userId = GetUserId();
        if (room.Members.TryGetValue(userId, out var member))
        {
            member.IsReady = !member.IsReady;
            await _splitRoomRepository.SetRoomAsync(room);
            await Clients.Group(roomId).ReceiveRoomUpdate(room);
        }
    }

    /// Xử lý sự kiện ngắt kết nối (Tắt tab/Mất mạng)
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}