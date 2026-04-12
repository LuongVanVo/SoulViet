using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetVoucherById;

public class GetVoucherByIdValidator : AbstractValidator<GetVoucherByIdQuery>
{
    public GetVoucherByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Voucher ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid voucher ID format.");
    }
}