using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.UpdateCartItem;

public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid User ID format.");

        RuleFor(x => x.CartItemId)
            .NotEmpty().WithMessage("CartItemId is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Cart Item ID format.");

        RuleFor(x => x.NewQuantity)
            .GreaterThanOrEqualTo(1).WithMessage("Quantity must be greater than zero.");
    }
}