using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.CheckValidVoucher;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetAvailableVouchers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Marketplace.Marketplace.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/tourist/vouchers")]
public class TouristVoucherController : ControllerBase
{
    private readonly IMediator _mediator;
    public TouristVoucherController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("available")]
    [SwaggerOperation(Summary = "Get available vouchers for tourist",
        Description = "Retrieves a list of vouchers that are currently available for the tourist to use.")]
    public async Task<IActionResult> GetAvailableVouchers([FromQuery] GetAvailableVouchersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("check-valid")]
    [SwaggerOperation(Summary = "Check if a voucher code is valid for the tourist",
        Description = "Checks whether a given voucher code is valid and can be applied to the tourist's cart.")]
    public async Task<IActionResult> CheckValidVoucher([FromQuery] CheckValidVoucherQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}