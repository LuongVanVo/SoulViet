using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.ToggleVoucherStatus;

public class ToggleVoucherStatusValidator : AbstractValidator<ToggleVoucherStatusCommand>
{
    public ToggleVoucherStatusValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Voucher ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid voucher ID format.");
    }
}