using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.AddToCart;

public class AddToCartValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid User ID format.");

        RuleFor(x => x.MarketplaceProductId)
            .NotEmpty().WithMessage("MarketplaceProductId is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Marketplace Product ID format.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(1).WithMessage("Quantity must be greater than zero.");
    }
}