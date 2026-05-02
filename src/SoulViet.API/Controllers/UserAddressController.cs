using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.API.Helper;
using SoulViet.Shared.Application.Features.UserAddressBook.Commands.CreateUserAddress;
using SoulViet.Shared.Application.Features.UserAddressBook.Commands.DeleteUserAddress;
using SoulViet.Shared.Application.Features.UserAddressBook.Commands.SetDefaultAddress;
using SoulViet.Shared.Application.Features.UserAddressBook.Commands.UpdateUserAddress;
using SoulViet.Shared.Application.Features.UserAddressBook.Queries.GetUserAddresses;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.API.Controllers;

[Authorize]
[ApiController]
[Route("api/user-address")]
public class UserAddressController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserAddressController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new user address", Description = "Create a new user address for the current authenticated user.")]
    public async Task<IActionResult> CreateUserAddress([FromBody] CreateUserAddressCommand command,
        CancellationToken cancellationToken)
    {
        command.UserId = User.GetCurrentUserId();
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get user addresses",
        Description = "Get all user addresses of the current authenticated user.")]
    public async Task<IActionResult> GetUserAddresses(CancellationToken cancellationToken)
    {
        var query = new GetUserAddressesQuery();
        query.UserId = User.GetCurrentUserId();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id}")]
    [SwaggerOperation(Summary = "Update a user address",
        Description = "Update a user address of the current authenticated user.")]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateUserAddressCommand command)
    {
        command.Id = id;
        command.UserId = User.GetCurrentUserId();

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch("{id}/set-default")]
    [SwaggerOperation(Summary = "Set default user address",
        Description = "Set a user address as default for the current authenticated user.")]
    public async Task<IActionResult> SetDefaultUserAddress([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new SetDefaultAddressCommand
        {
            Id = id,
            UserId = User.GetCurrentUserId()
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a user address",
        Description = "Delete a user address of the current authenticated user.")]
    public async Task<IActionResult> DeleteUserAddress(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserAddressCommand
        {
            Id = id,
            UserId = User.GetCurrentUserId()
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}