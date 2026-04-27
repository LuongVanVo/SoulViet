using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Queries.GetPayoutBatchDetails;

public class GetPayoutBatchDetailsValidator : AbstractValidator<GetPayoutBatchDetailsQuery>
{
    public GetPayoutBatchDetailsValidator()
    {
        RuleFor(x => x.PayoutBatchId)
            .NotEmpty().WithMessage("Payout batch ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid payout batch ID format.");
    }
}