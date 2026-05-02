namespace SoulViet.Shared.Application.DTOs;

public class UserAddressDto
{
    public Guid Id { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? District { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string DetailedAddress { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    public string FullAddress
    {
        get
        {
            var parts = new List<string?> { DetailedAddress, Ward, District, Province };
            return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }
}