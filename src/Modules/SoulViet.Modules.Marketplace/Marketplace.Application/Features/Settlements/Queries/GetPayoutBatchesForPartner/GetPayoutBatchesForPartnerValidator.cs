using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Queries.GetPayoutBatchesForPartner;

public class GetPayoutBatchesForPartnerValidator : AbstractValidator<GetPayoutBatchesForPartnerQuery>
{
    public GetPayoutBatchesForPartnerValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be less than or equal to 100.");
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid payout batch status.");
    }
}