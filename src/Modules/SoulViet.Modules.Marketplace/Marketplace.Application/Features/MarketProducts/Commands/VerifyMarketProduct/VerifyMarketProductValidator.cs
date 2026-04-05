using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.VerifyMarketProduct;

public class VerifyMarketProductValidator : AbstractValidator<VerifyMarketProductCommand>
{
    public VerifyMarketProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Product ID format.");
    }
}