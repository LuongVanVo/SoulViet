using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Domain.Entities
{
    public class PostMedia
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PostId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string ObjectKey { get; set; } = string.Empty;

        public MediaType MediaType { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public long? FileSizeBytes { get; set; }
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
