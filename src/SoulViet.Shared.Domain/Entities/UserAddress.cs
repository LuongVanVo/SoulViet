using SoulViet.Shared.Domain.Common;

namespace SoulViet.Shared.Domain.Entities;

public class UserAddress : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;

    public string Province { get; set; } = string.Empty;
    public string? District { get; set; } = string.Empty; // Có thể null vì sáp nhập tỉnh/thành phố còn 2 cấp
    public string Ward { get; set; } = string.Empty;
    public string DetailedAddress { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false; // Địa chỉ mặc định
}