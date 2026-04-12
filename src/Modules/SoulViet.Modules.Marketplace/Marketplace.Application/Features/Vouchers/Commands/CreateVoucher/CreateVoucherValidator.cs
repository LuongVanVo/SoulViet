using FluentValidation;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.CreateVoucher;

public class CreateVoucherValidator : AbstractValidator<CreateVoucherCommand>
{
    public CreateVoucherValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Voucher code is required.")
            .MaximumLength(50).WithMessage("Voucher code must not exceed 50 characters.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be greater than start date.")
            .GreaterThan(DateTime.UtcNow).WithMessage("End date must be in the future.");

        RuleFor(x => x.PartnerId)
            .NotNull().When(x => x.Scope == VoucherScope.Shop)
            .WithMessage("PartnerId is required when creating a Shop voucher.");

        RuleFor(x => x.PartnerId)
            .Null().When(x => x.Scope == VoucherScope.Platform)
            .WithMessage("Platform voucher cannot be assigned to a specific Partner.");

        RuleFor(x => x.DiscountValue)
            .GreaterThan(0).When(x => x.DiscountType != DiscountType.FreeShipping)
            .WithMessage("Discount value must be greater than 0.");

        RuleFor(x => x.DiscountValue)
            .Equal(0).When(x => x.DiscountType == DiscountType.FreeShipping)
            .WithMessage("Discount value must be 0 for Free Shipping vouchers.");

        When(x => x.DiscountType == DiscountType.Percentage, () =>
        {
            RuleFor(x => x.DiscountValue)
                .LessThanOrEqualTo(100).WithMessage("Percentage discount value must be between 1 and 100.");

            RuleFor(x => x.MaxDiscountAmount)
                .NotNull().WithMessage("MaxDiscountAmount is required for Percentage discount.")
                .GreaterThan(0).WithMessage("MaxDiscountAmount must be greater than 0.");
        });

        When(x => x.DiscountType == DiscountType.FixedAmount, () =>
        {
            RuleFor(x => x.DiscountValue)
                .LessThanOrEqualTo(x => x.MinOrderAmount)
                .WithMessage("Discount value cannot be greater than the minimum order amount to prevent negative totals.");
        });

        When(x => x.DiscountType == DiscountType.FreeShipping, () =>
        {
            RuleFor(x => x.MaxDiscountAmount)
                .GreaterThan(0).When(x => x.MaxDiscountAmount.HasValue)
                .WithMessage("Max discount amount for Free Shipping must be greater than 0 if provided.");
        });

        RuleFor(x => x.UsageLimit)
            .GreaterThanOrEqualTo(1).WithMessage("Usage limit must be at least 1.");

        RuleFor(x => x.MinOrderAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum order amount cannot be negative.");
    }
}