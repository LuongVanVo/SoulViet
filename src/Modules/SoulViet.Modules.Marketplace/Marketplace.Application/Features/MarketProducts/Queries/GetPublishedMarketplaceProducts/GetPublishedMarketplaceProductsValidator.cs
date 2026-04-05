using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetPublishedMarketplaceProducts;

public class GetPublishedMarketplaceProductsValidator : AbstractValidator<GetPublishedMarketplaceProductsQuery>
{
    public GetPublishedMarketplaceProductsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not be exceed 100.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum price must be greater than or equal to 0.")
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Maximum price must be greater than or equal to 0.")
            .GreaterThanOrEqualTo(x=> x.MinPrice).WithMessage("Maximum price must be greater than or equal to minimum price.")
            .When(x => x.MaxPrice.HasValue && x.MinPrice.HasValue);
    }
}