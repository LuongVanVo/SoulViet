using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Commands.Results;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Commands.LockAndGenerateLinks;

public class LockAndGenerateLinksCommand : IRequest<LockAndGenerateLinksResponse>
{
    public string RoomId { get; set; } = String.Empty;
    public Guid HostUserId { get; set; }
}