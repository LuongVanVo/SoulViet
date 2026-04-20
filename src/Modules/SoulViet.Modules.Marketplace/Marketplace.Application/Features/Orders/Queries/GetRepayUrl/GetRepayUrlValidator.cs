using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetRepayUrl;

public class GetRepayUrlValidator : AbstractValidator<GetRepayUrlQuery>
{
    public GetRepayUrlValidator()
    {
        RuleFor(x => x.MasterOrderId)
            .NotEmpty().WithMessage("MasterOrderId is required.")
            .Must(id => id != Guid.Empty).WithMessage("MasterOrderId must be a valid GUID.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(id => id != Guid.Empty).WithMessage("UserId must be a valid GUID.");
    }
}