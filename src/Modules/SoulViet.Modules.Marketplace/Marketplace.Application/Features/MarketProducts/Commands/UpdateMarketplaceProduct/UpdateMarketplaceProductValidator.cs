using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Commands.UpdateMarketplaceProduct;

public class UpdateMarketplaceProductValidator : AbstractValidator<UpdateMarketplaceProductCommand>
{
    public UpdateMarketplaceProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Product ID format.");

        RuleFor(x => x.PartnerId)
            .NotEmpty().WithMessage("Product ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Partner ID format.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Product ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Category ID format.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(255).WithMessage("Product name must not exceed 255 characters.");

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