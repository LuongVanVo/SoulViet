using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Application.DTOs
{
    public class PostShareDto
    {
        public string? Caption { get; set; }
        public ShareType ShareType { get; set; } = ShareType.Timeline;
    }
}
