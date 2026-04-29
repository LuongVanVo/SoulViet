using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Shared.Application.Features.LocalPartners.Queries.GetLocalpartnerByUserId;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.API.Controllers;

[ApiController]
[Route("api/local-partners")]
public class LocalPartnerController : ControllerBase
{
    private readonly IMediator _mediator;
    public LocalPartnerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{localPartnerId}")]
    [SwaggerOperation(Summary = "Get local partner information by user ID",
        Description = "Retrieves local partner information based on the associated user ID.")]
    public async Task<IActionResult> GetLocalPartnerByUserId([FromRoute] Guid localPartnerId)
    {
        var query = new GetLocalPartnerByUserIdQuery
        {
            UserId = localPartnerId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}