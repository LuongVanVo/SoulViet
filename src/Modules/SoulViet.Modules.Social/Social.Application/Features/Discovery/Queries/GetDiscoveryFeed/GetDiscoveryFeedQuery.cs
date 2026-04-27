using MediatR;
using SoulViet.Modules.Social.Social.Application.Common.Pagination;
using SoulViet.Modules.Social.Social.Application.Features.Posts.Results;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Application.Features.Discovery.Queries.GetDiscoveryFeed
{
    public class GetDiscoveryFeedQuery : IRequest<Connection<PostResponse>>
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double RadiusKm { get; set; } = 10; // Mặc định 10km
        public VibeTag? VibeTag { get; set; }
        public string SortBy { get; set; } = "trending"; // trending, nearby, newest
        
        public string? After { get; set; }
        public int First { get; set; } = 10;
    }
}
