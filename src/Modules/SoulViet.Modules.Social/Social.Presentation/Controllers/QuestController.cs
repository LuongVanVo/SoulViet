using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Shared.Application.Interfaces;
using System.Security.Claims;

namespace SoulViet.Modules.Social.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/users/quests")]
public class QuestController : ControllerBase
{
    private readonly ISocialQuestService _socialQuestService;

    public QuestController(ISocialQuestService socialQuestService)
    {
        _socialQuestService = socialQuestService;
    }

    [HttpGet("daily-progress")]
    public async Task<IActionResult> GetDailyProgress()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var progress = await _socialQuestService.GetDailyQuestProgressAsync(userId);
        return Ok(progress);
    }
}
