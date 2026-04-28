using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Commands.LockAndGenerateLinks;
using SoulViet.Modules.Marketplace.Marketplace.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Marketplace.Marketplace.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/marketplace/bill-split")]
public class BillSplitController : ControllerBase
{
    private readonly IMediator _mediator;

    public BillSplitController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Khóa phòng và sinh link thanh toán cho từng thành viên.
    /// </summary>
    /// <remarks>
    /// Chỉ Trưởng nhóm (Host) mới có quyền thực hiện hành động này.
    /// Hệ thống sẽ kiểm tra xem tổng tiền đã khớp chưa và các thành viên đã sẵn sàng chưa trước khi sinh link VNPay.
    /// </remarks>
    [HttpPost("lock")]
    [SwaggerOperation(
        Summary = "Lock split room and generate payment links",
        Description = "Locks the room to prevent further changes and generates individual VNPay payment links for each member.")]
    public async Task<IActionResult> LockRoom([FromBody] LockAndGenerateLinksCommand request, CancellationToken cancellationToken)
    {
        var command = new LockAndGenerateLinksCommand
        {
            RoomId = request.RoomId,
            HostUserId = User.GetCurrentUserId()
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}