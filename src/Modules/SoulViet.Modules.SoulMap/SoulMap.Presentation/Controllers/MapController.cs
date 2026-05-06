using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetAccommodationDetail;
using SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetMapMarkersByIds;
using SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetNearbyPlaces;
using SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetPlacesByVibeTag;
using SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetPlacesInBounds;
using SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetTouristAttractionDetail;
using SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetTrendingPlaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.SoulMap.SoulMap.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MapController : ControllerBase
{
    private readonly IMediator _mediator;
    public MapController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("nearby")]
    [SwaggerOperation(Summary = "Get nearby places based on user's location",
        Description = "Returns a list of nearby places based on the user's current location and specified radius.")]
    public async Task<IActionResult> GetNearbyPlaces([FromQuery] GetNearbyPlacesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("trending")]
    [SwaggerOperation(Summary = "Get trending places in a province",
        Description =
            "Returns a list of trending places in a specified province, filtered by mode (tourist, accommodation, or all).")]
    public async Task<IActionResult> GetTrendingPlaces([FromQuery] GetTrendingPlacesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("bounds")]
    [SwaggerOperation(Summary = "Get places within map bounds",
        Description =
            "Returns a list of places that fall within the specified map bounds, filtered by mode (tourist, accommodation, or all).")]
    public async Task<IActionResult> GetPlacesWithinBounds([FromQuery] GetPlacesInBoundsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("markers")]
    [SwaggerOperation(Summary = "Get list places from list of place IDs",
        Description =
            "Returns a list of places based on the provided list of place IDs, filtered by mode (tourist, accommodation, or all).")]
    public async Task<IActionResult> GetPlacesByIds([FromQuery] GetMapMarkersByIdsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("tourist-attraction/{id}")]
    [SwaggerOperation(Summary = "Get tourist attraction detail by ID",
        Description = "Returns detailed information about a specific tourist attraction based on its ID.")]
    public async Task<IActionResult> GetTouristAttractionDetail([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetTouristAttractionDetailQuery() { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("accommodation/{id}")]
    [SwaggerOperation(Summary = "Get accommodation detail by ID",
        Description = "Returns detailed information about a specific accommodation based on its ID.")]
    public async Task<IActionResult> GetAccommodationDetail([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetAccommodationDetailQuery{ Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("vibe-tag")]
    [SwaggerOperation(Summary = "Get places by vibe tag",
        Description =
            "Returns a list of places that match the specified vibe tag, optionally filtered by province ID and mode (tourist, accommodation, or all).")]
    public async Task<IActionResult> GetPlacesByVibeTag([FromQuery] GetPlacesByVibeTagQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}