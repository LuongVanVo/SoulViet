namespace SoulViet.Shared.Application.Common.Events;

public class SoulCoinDeductedEvent
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public DateTime DeductedAt { get; set; } = DateTime.UtcNow;
}
