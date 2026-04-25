using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Social.Social.Application.Features.Discovery.Queries.GetDiscoveryFeed;
using SoulViet.Shared.Domain.Enums;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscoveryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DiscoveryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetDiscoveryFeed(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] double radiusKm = 10,
            [FromQuery] VibeTag? vibeTag = null,
            [FromQuery] string sortBy = "trending",
            [FromQuery] string? after = null,
            [FromQuery] int first = 10)
        {
            var query = new GetDiscoveryFeedQuery
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusKm = radiusKm,
                VibeTag = vibeTag,
                SortBy = sortBy,
                After = after,
                First = first
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
