namespace SoulViet.Shared.Application.Features.Auth.Results;

public class UserProfileResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int PostsCount { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsFollower { get; set; }
    public bool IsLocalPartner { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}