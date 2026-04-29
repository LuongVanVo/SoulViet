namespace SoulViet.Modules.Social.Social.Domain.Entities
{
    public class PostLike
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Post Post { get; set; } = null!;
    }
}