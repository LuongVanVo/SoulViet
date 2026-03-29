using FluentValidation;

namespace SoulViet.Shared.Application.Features.Auth.Queries.GetUserProfile;

public class GetUserProfileValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("User ID cannot be an empty GUID.");
    }
}