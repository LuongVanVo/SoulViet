using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Commands.GeneratePayoutBatches;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Queries.GetPayoutBatchDetails;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Queries.GetPayoutBatchesForAdmin;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Queries.GetPayoutBatchesForPartner;
using SoulViet.Modules.Marketplace.Marketplace.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Marketplace.Marketplace.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettlementsController : ControllerBase
{
    private readonly IMediator _mediator;
    public SettlementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("generate-batches")]
    [SwaggerOperation(Summary = "Generate payout batches for a specified period. This is typically called by a scheduled job.", Description = "This endpoint generates payout batches based on completed orders within a specified date range. It is intended to be called by a scheduled job (e.g., daily or weekly) to automate the settlement process. The command accepts an optional end date, and if not provided, it defaults to the current date and time.")]
    public async Task<IActionResult> GenerateBatches([FromBody] GeneratePayoutBatchesCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/batches")]
    [SwaggerOperation(Summary = "Get paginated payout batches for admin view",
        Description =
            "This endpoint retrieves a paginated list of payout batches for administrative purposes. It supports optional filtering by partner name and settlement status, allowing admins to easily manage and review payout batches.")]
    public async Task<IActionResult> GetBatchesForAdmin([FromQuery] GetPayoutBatchesForAdminQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "LocalPartner")]
    [HttpGet("partner/batches")]
    [SwaggerOperation(Summary = "Get paginated payout batches for partner view",
        Description =
            "This endpoint retrieves a paginated list of payout batches for the currently authenticated partner. It supports optional filtering by settlement status, allowing partners to easily track their payout batches and manage their settlements.")]
    public async Task<IActionResult> GetBatchesForPartner([FromQuery] GetPayoutBatchesForPartnerQuery query,
        CancellationToken cancellationToken)
    {
        query.PartnerId = User.GetCurrentUserId();

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("batches/{PayoutBatchId}")]
    [SwaggerOperation(Summary = "Get details of a specific payout batch",
        Description =
            "This endpoint retrieves detailed information about a specific payout batch, including the list of orders included in the batch. It is accessible to both admins and partners, but partners can only access batches that belong to them.")]
    public async Task<IActionResult> GetBatchDetails([FromRoute] GetPayoutBatchDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}