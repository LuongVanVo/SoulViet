using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.CreateMarketplaceProduct;

public class CreateMarketplaceProductValidator : AbstractValidator<CreateMarketplaceProductCommand>
{
    public CreateMarketplaceProductValidator()
    {
        RuleFor(x => x.PartnerId)
            .NotEmpty().WithMessage("Category ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Partner ID format.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Category ID format.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(255).WithMessage("Product name must not exceed 255 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required.")
            .MaximumLength(4000).WithMessage("Product description must not exceed 4000 characters.");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Stock)
            .NotEmpty().WithMessage("Stock is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

        RuleFor(x => x.ProductType)
            .IsInEnum().WithMessage("Invalid product type.");
    }
}