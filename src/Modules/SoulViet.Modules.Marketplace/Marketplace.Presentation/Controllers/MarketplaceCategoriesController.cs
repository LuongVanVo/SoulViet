using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Commands.CreateCategory;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Queries.GetAllCategories;
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

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new marketplace category", Description = "Creates a new category for marketplace products. Requires Admin role.")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all active marketplace categories",
        Description = "Retrieves a list of all active marketplace categories.")]
    public async Task<IActionResult> GetAllActiveCategories()
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery());

        return Ok(result);
    }
}