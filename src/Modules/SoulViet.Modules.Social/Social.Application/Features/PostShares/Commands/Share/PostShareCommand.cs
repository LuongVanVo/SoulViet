using MediatR;
using SoulViet.Modules.Social.Social.Application.Features.PostShares.Results;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.Social.Social.Application.Features.PostShares.Commands.Share
{
    public record PostShareCommand(
        Guid PostId, 
        Guid UserId, 
        string UserName, 
        string Caption,
        ShareType ShareType
    ) : IRequest<PostShareResult>;
}
