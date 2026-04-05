using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.DeleteMarketplaceProduct;

public class DeleteMarketplaceProductValidator : AbstractValidator<DeleteMarketplaceProductCommand>
{
    public DeleteMarketplaceProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid product ID format.");

        RuleFor(x => x.PartnerId)
            .NotEmpty().WithMessage("Partner ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Partner ID format.");
    }
}