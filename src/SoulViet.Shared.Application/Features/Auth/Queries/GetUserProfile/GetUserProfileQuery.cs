using MediatR;
using SoulViet.Shared.Application.Features.Auth.Results;

namespace SoulViet.Shared.Application.Features.Auth.Queries.GetUserProfile;

public class GetUserProfileQuery : IRequest<UserProfileResponse>
{
    public Guid UserId { get; set; }
}