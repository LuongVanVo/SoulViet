using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetAvailableVouchers;

public class GetAvailableVouchersValidator : AbstractValidator<GetAvailableVouchersQuery>
{
    public GetAvailableVouchersValidator()
    {
        RuleFor(x => x.CurrentOrderAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Current order amount must be non-negative.");
    }
}