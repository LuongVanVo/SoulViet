namespace SoulViet.Shared.Application.Common.Events;

public class SoulCoinEarnedEvent
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
}
