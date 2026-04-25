namespace SoulViet.Modules.Social.Social.Application.DTOs
{
    public class FollowerUserDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsFollowing { get; set; } // If the current user follows this person
    }
}
