using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.CheckValidVoucher;

public class CheckValidVoucherValidator : AbstractValidator<CheckValidVoucherQuery>
{
    public CheckValidVoucherValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Voucher code is required.")
            .MaximumLength(50).WithMessage("Voucher code must not exceed 50 characters.");

        RuleFor(x => x.CurrentOrderAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Current order amount must be greater than or equal to 0.");
    }
}