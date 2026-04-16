using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.CreateOrder;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.PreviewOrder;
using SoulViet.Modules.Marketplace.Marketplace.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Marketplace.Marketplace.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/marketplace/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("preview")]
    [SwaggerOperation(Summary = "Preview order details",
        Description =
            "Previews the order details based on the selected cart items and applied vouchers before placing the order.")]
    public async Task<IActionResult> PreviewOrder([FromBody] PreviewOrderQuery query,
        CancellationToken cancellationToken)
    {
        var userId = User.GetCurrentUserId();
        query.UserId = userId;

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new order",
        Description = "Creates a new order based on the selected cart items and applied vouchers.")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        command.UserId = User.GetCurrentUserId();

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}