namespace SoulViet.Modules.Social.Social.Domain.Entities
{
    public class UserFollower
    {
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}