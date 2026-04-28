namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Models;

public class SplitMemberState
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;

    // ConnectionId of SignalR to know this member is online or offline
    public string ConnectionId { get; set; } = string.Empty;

    public decimal AmountToPay { get; set; } // Số tiền mà thành viên này cần phải thanh toán
    public bool IsReady { get; set; } = false; // Đánh dấu đã sẵn sàng thanh toán hay chưa (chốt hay chưa )
    public bool HasPaid { get; set; } = false; // Đánh dấu đã thanh toán thành công hay chưa

    // Link thanh toán riêng của thành viên này.
    public string? PaymentUrl { get; set; }
}