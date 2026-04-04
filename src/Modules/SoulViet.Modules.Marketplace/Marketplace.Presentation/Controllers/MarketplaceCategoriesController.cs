using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Commands.CreateCategory;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Commands.DeleteCategory;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Commands.UpdateCategory;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Queries.GetAllCategories;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Queries.GetCategoryById;
using SoulViet.Modules.Marketplace.Marketplace.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Marketplace.Marketplace.Presentation.Controllers;

[ApiController]
[Route("api/marketplace/categories")]
public class MarketplaceCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public MarketplaceCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Create a new marketplace category (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new marketplace category", Description = "Creates a new category for marketplace products. Requires Admin role.")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        command.CreatedBy = User.GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // Get all active marketplace categories
    [HttpGet]
    [SwaggerOperation(Summary = "Get all active marketplace categories",
        Description = "Retrieves a list of all active marketplace categories.")]
    public async Task<IActionResult> GetAllActiveCategories()
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery());

        return Ok(result);
    }

    // Get a marketplace category by ID
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get category by ID", Description = "Retrieves a marketplace category by its ID.")]
    public async Task<IActionResult> GetCategoryById([FromRoute] GetCategoryByIdQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // Update marketplace category by id
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    [SwaggerOperation(Summary = "Update category by ID",
        Description = "Updates a marketplace category by its ID. Requires Admin role.")]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        command.ModifiedBy = User.GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // Delete marketplace category by id (soft delete)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete category by ID",
        Description = "Soft deletes a marketplace category by its ID. Requires Admin role.")]
    public async Task<IActionResult> DeleteCategory([FromRoute] DeleteCategoryCommand command)
    {
        command.Id = command.Id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}