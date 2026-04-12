using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.CreateVoucher;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.ToggleVoucherStatus;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.UpdateVoucher;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetVoucherById;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetVouchersWithPagination;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Marketplace.Marketplace.Presentation.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/[controller]")]
public class AdminVoucherController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminVoucherController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a Platform voucher", Description = "Admin creates a voucher that applies to the entire platform.")]
    public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherCommand command)
    {
        command.Scope = VoucherScope.Platform;
        command.PartnerId = null;

        var result = await _mediator.Send(command);
        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all vouchers",
        Description = "Admin retrieves a list of all vouchers, including both platform and shop-specific vouchers.")]
    public async Task<IActionResult> GetVouchers([FromQuery] GetVouchersWithPaginationQuery query)
    {
        query.IsAdmin = true;
        query.PartnerId = null;

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPatch("{id}/toggle-status")]
    [SwaggerOperation(Summary = "Toggle voucher status",
        Description = "Admin toggles the active status of a voucher by its ID.")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var command = new ToggleVoucherStatusCommand
        {
            Id = id,
            IsAdmin = true
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch("{id}")]
    [SwaggerOperation(Summary = "Update a voucher",
        Description = "Admin updates the details of a voucher by its ID.")]
    public async Task<IActionResult> UpdateVoucher([FromRoute] Guid id, [FromBody] UpdateVoucherCommand command)
    {
        command.Id = id;
        command.IsAdmin = true;
        command.PartnerId = null;

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get voucher details",
        Description = "Admin retrieves the details of a specific voucher by its ID.")]
    public async Task<IActionResult> GetVoucherDetails([FromRoute] Guid id)
    {
        var query = new GetVoucherByIdQuery
        {
            Id = id,
            IsAdmin = true,
            PartnerId = null
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}