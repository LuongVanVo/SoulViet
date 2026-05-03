using System;

namespace SoulViet.Modules.Social.Social.Application.DTOs
{
    public class PostLikerDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsLocalPartner { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsFollower { get; set; }
    }
}
