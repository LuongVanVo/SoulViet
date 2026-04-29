using FluentValidation;

namespace SoulViet.Shared.Application.Features.LocalPartners.Queries.GetLocalpartnerByUserId;

public class GetLocalPartnerByUserIdValidator : AbstractValidator<GetLocalPartnerByUserIdQuery>
{
    public GetLocalPartnerByUserIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(userId => userId != Guid.Empty).WithMessage("UserId must be a valid GUID.");
    }
}