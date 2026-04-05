using System.Data;
using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetMyMarketplaceProducts;

public class GetMyMarketplaceProductsValidator : AbstractValidator<GetMyMarketplaceProductsQuery>
{
    public GetMyMarketplaceProductsValidator()
    {
        RuleFor(x => x.PartnerId)
            .NotEmpty().WithMessage("Partner ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Partner ID format.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");
    }
}