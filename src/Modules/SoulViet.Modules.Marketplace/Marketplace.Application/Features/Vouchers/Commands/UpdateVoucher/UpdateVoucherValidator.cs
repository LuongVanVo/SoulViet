using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.UpdateVoucher;

public class UpdateVoucherValidator : AbstractValidator<UpdateVoucherCommand>
{
    public UpdateVoucherValidator()
    {
        RuleFor(x => x.EndDate)
            .GreaterThan(DateTime.UtcNow).When(x => x.EndDate.HasValue)
            .WithMessage("End date must be in the future.");

        RuleFor(x => x.UsageLimit)
            .GreaterThanOrEqualTo(1).When(x => x.UsageLimit.HasValue)
            .WithMessage("Usage limit must be at least 1.");

        RuleFor(x => x.MinOrderAmount)
            .GreaterThanOrEqualTo(0).When(x => x.MinOrderAmount.HasValue)
            .WithMessage("Minimum order amount cannot be negative.");

        RuleFor(x => x.MaxDiscountAmount)
            .GreaterThan(0).When(x => x.MaxDiscountAmount.HasValue)
            .WithMessage("Max discount amount must be greater than 0 if provided.");
    }
}