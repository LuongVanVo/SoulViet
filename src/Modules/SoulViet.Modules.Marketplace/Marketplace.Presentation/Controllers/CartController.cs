using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.AddToCart;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.ClearCart;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.RemoveCartItems;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.UpdateCartItem;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Queries.GetMyCart;
using SoulViet.Modules.Marketplace.Marketplace.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Marketplace.Marketplace.Presentation.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Add item to cart",
        Description = "Adds a specified quantity of a marketplace product to the user's cart.")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartCommand command, CancellationToken cancellationToken)
    {
        command.UserId = User.GetCurrentUserId();
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get my cart",
        Description = "Retrieves the current user's cart with all items and their details.")]
    public async Task<IActionResult> GetMyCart([FromRoute] GetMyCartQuery query, CancellationToken cancellationToken)
    {
        query.UserId = User.GetCurrentUserId();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("items/{id}")]
    [SwaggerOperation(Summary = "Update cart item quantity",
        Description = "Updates the quantity of a specific item in the user's cart.")]
    public async Task<IActionResult> UpdateCartItem([FromRoute] Guid id, [FromBody] UpdateCartItemCommand command,
        CancellationToken cancellationToken)
    {
        command.UserId = User.GetCurrentUserId();
        command.CartItemId = id;
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("items")]
    [SwaggerOperation(Summary = "Remove items from cart",
        Description = "Removes specified items from the user's cart.")]
    public async Task<IActionResult> RemoveCartItems([FromBody] RemoveCartItemsCommand command, CancellationToken cancellationToken)
    {
        command.UserId = User.GetCurrentUserId();
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("clear")]
    [SwaggerOperation(Summary = "Clear cart",
        Description = "Removes all items from the user's cart.")]
    public async Task<IActionResult> ClearCart() {
        var command = new ClearCartCommand
        {
            UserId = User.GetCurrentUserId()
        };
        var result = await _mediator.Send(command, CancellationToken.None);
        return Ok(result);
    }
}