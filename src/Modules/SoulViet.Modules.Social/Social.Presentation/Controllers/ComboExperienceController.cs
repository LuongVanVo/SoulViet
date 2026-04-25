using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Social.Social.Application.Features.ComboExperiences.Commands.CreateComboExperience;
using SoulViet.Modules.Social.Social.Application.Features.ComboExperiences.Commands.UpdateComboExperience;
using SoulViet.Modules.Social.Social.Application.Features.ComboExperiences.Commands.DeleteComboExperience;
// Gọi Extension Method lấy UserId (Giống như bên PostController của bạn)
using SoulViet.Modules.Social.Social.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Social.Social.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ComboExperienceController : ControllerBase 
    {
        private readonly IMediator _mediator;

        public ComboExperienceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new Combo Experience", Description = "Allows Local Guides to create a new combo.")]
        public async Task<IActionResult> Create(
            [FromBody] CreateComboExperienceCommand command,
            CancellationToken cancellationToken)
        {
            command.GuideId = User.GetCurrentUserId();

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation(Summary = "Update an existing Combo", Description = "Local Guides can update their own combo details.")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateComboExperienceCommand command,
            CancellationToken cancellationToken)
        {
            command.Id = id;
            command.GuideId = User.GetCurrentUserId();

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Delete a Combo Experience", Description = "Soft-deletes a combo owned by the Local Guide.")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteComboExperienceCommand
            {
                Id = id,
                GuideId = User.GetCurrentUserId()
            };

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
