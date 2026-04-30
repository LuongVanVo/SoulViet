using MediatR;
using SoulViet.Shared.Application.Features.Auth.Results;

namespace SoulViet.Modules.Social.Social.Application.Features.Users.Queries.GetPublicProfile
{
    public class GetPublicProfileQuery : IRequest<UserProfileResponse>
    {
        public Guid UserId { get; set; }
        public Guid? CurrentUserId { get; set; }
    }
}
