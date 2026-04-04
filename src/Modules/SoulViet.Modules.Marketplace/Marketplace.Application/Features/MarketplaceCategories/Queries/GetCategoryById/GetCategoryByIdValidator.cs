using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Queries.GetCategoryById;

public class GetCategoryByIdValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Category ID format.");
    }
}