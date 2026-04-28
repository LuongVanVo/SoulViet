namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Models;

public class SplitRoomSession
{
    public string RoomId { get; set; } = string.Empty;
    public Guid MasterOrderId { get; set; }
    public Guid HostUserId { get; set; }

    public decimal TotalAmount { get; set; } // Tổng bill phải trả
    public string SplitType { get; set; } = "Equally";
    public bool IsLocked { get; set; } = false;

    // Danh sách thành viên
    public Dictionary<Guid, SplitMemberState> Members { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsAmountValid() => Members.Values.Sum(m => m.AmountToPay) == TotalAmount;
}