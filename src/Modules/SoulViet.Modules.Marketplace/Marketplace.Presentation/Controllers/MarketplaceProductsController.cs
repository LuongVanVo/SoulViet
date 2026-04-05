using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.CreateMarketplaceProduct;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.DeleteMarketplaceProduct;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.UpdateMarketplaceProduct;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.VerifyMarketProduct;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetMarketplaceProductById;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetMarketplaceProductsForAdmin;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetMyMarketplaceProducts;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetPublishedMarketplaceProducts;
using SoulViet.Modules.Marketplace.Marketplace.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Marketplace.Marketplace.Presentation.Controllers;

[ApiController]
[Route("api/marketplace/products")]
public class MarketplaceProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public MarketplaceProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "LocalPartner")]
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new marketplace product", Description = "Creates a new product for the marketplace. Requires LocalPartner role.")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateMarketplaceProductCommand command)
    {
        command.PartnerId = User.GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get marketplace product by ID",
        Description = "Retrieves a marketplace product by its unique identifier.")]
    public async Task<IActionResult> GetProductById([FromRoute] GetMarketplaceProductByIdQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/verify")]
    [SwaggerOperation(Summary = "Verify a marketplace product",
        Description = "Marks a marketplace product as verified by an admin. Requires Admin role.")]
    public async Task<IActionResult> VerifyProductByAdmin([FromRoute] VerifyMarketProductCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [Authorize(Roles = "LocalPartner")]
    [HttpPatch("{id}")]
    [SwaggerOperation(Summary = "Update a marketplace product",
        Description = "Updates the details of a marketplace product. Requires Admin or LocalPartner role.")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id,
        [FromBody] UpdateMarketplaceProductCommand command)
    {
        command.Id = id;
        command.PartnerId = User.GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [Authorize(Roles = "LocalPartner")]
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a marketplace product",
        Description = "Soft deletes a marketplace product. Requires LocalPartner role.")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        var command = new DeleteMarketplaceProductCommand
        {
            Id = id,
            PartnerId = User.GetCurrentUserId()
        };

        var result = await _mediator.Send(command);
        return Ok(new
        {
            Success = result,
            Message = result ? "Product deleted successfully." : "Failed to delete product."
        });
    }

    [Authorize(Roles = "LocalPartner")]
    [HttpGet("partner")]
    [SwaggerOperation(Summary = "Get products by partner ID",
        Description =
            "Retrieves a paginated list of marketplace products for a specific partner. Requires LocalPartner role.")]
    public async Task<IActionResult> GetProductsOfPartner([FromQuery] GetMyMarketplaceProductsQuery query)
    {
        query.PartnerId = User.GetCurrentUserId();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    [SwaggerOperation(Summary = "Get products for admin",
        Description =
            "Retrieves a paginated list of marketplace products for admin view. Requires Admin role.")]
    public async Task<IActionResult> GetProductsForAdmin([FromQuery] GetMarketplaceProductsForAdminQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("tourists")]
    [SwaggerOperation(Summary = "Get published products for tourists",
        Description =
            "Retrieves a paginated list of published marketplace products for tourists. No authentication required.")]
    public async Task<IActionResult> GetPublishedProductsForTourists(
        [FromQuery] GetPublishedMarketplaceProductsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}