using MediatR;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Features.Auth.Results;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Shared.Application.Features.Auth.Queries.GetUserProfile;

public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserProfileResponse>
{
    private readonly IUserRepository _userRepository;
    public GetUserProfileHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        // find user by id
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException("User not found.");

        // get user roles
        var roles = await _userRepository.GetUserRolesAsync(user.Id);

        return new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            SoulCoinBalance = user.SoulCoinBalance
        };
    }
}