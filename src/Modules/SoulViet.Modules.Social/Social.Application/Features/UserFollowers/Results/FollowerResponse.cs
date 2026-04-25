namespace SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Results
{
    public class FollowerResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsFollowing { get; set; } 
    }
}
