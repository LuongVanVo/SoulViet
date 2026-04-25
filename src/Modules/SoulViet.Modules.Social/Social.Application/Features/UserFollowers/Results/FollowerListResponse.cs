using SoulViet.Modules.Social.Social.Application.DTOs;

namespace SoulViet.Modules.Social.Social.Application.Features.UserFollowers.Results
{
    public class FollowerListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<FollowerUserDto> Data { get; set; } = Enumerable.Empty<FollowerUserDto>();
    }
}
