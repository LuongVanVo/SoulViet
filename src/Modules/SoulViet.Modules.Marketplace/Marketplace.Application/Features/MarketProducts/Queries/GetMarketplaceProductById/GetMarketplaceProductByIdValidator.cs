using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetMarketplaceProductById;

public class GetMarketplaceProductByIdValidator : AbstractValidator<GetMarketplaceProductByIdQuery>
{
    public GetMarketplaceProductByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid product ID format.");
    }
}