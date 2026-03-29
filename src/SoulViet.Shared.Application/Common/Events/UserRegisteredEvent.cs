namespace SoulViet.Shared.Application.Common.Events;

public class UserRegisteredEvent
{
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}