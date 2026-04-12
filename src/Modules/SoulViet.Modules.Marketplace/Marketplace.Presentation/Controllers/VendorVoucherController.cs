using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.CreateVoucher;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.ToggleVoucherStatus;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.UpdateVoucher;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetVoucherById;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetVouchersWithPagination;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;
using SoulViet.Modules.Marketplace.Marketplace.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Marketplace.Marketplace.Presentation.Controllers;

[Authorize(Roles = "LocalPartner")]
[ApiController]
[Route("api/vendor/[controller]")]
public class VendorVoucherController : ControllerBase
{
    private readonly IMediator _mediator;
    public VendorVoucherController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a Shop voucher", Description = "Local Partner creates a voucher specific to their own shop.")]
    public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherCommand command)
    {
        command.Scope = VoucherScope.Shop;

        var userIdString = User.GetCurrentUserId().ToString();
        if (Guid.TryParse(userIdString, out Guid userId))
        {
            command.PartnerId = userId;
        }

        var result = await _mediator.Send(command);
        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get my vouchers",
        Description = "Local Partner retrieves a list of their own shop-specific vouchers.")]
    public async Task<IActionResult> GetVouchers([FromQuery] GetVouchersWithPaginationQuery query)
    {
        query.IsAdmin = false;
        query.PartnerId = User.GetCurrentUserId();

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [SwaggerOperation(Summary = "Toggle shop voucher status", Description = "Local Partner activates or deactivates their own voucher.")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var command = new ToggleVoucherStatusCommand()
        {
            Id = id,
            IsAdmin = false
        };

        command.PartnerId = User.GetCurrentUserId();

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch("{id}")]
    [SwaggerOperation(Summary = "Update voucher details for local partner",
        Description = "Allows the tourist to update certain details of a voucher, such as applying it to their cart or marking it as used.")]
    public async Task<IActionResult> UpdateVoucher([FromRoute] Guid id, [FromBody] UpdateVoucherCommand command)
    {
        command.Id = id;
        command.IsAdmin = false;
        command.PartnerId = User.GetCurrentUserId();

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get voucher details for local partner",
        Description = "Allows the tourist to retrieve the details of a specific voucher by its ID.")]
    public async Task<IActionResult> GetVoucherDetails([FromRoute] Guid id)
    {
        var query = new GetVoucherByIdQuery()
        {
            Id = id,
            IsAdmin = false,
            PartnerId = User.GetCurrentUserId()
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}