using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Queries.GetMyCart;

public class GetMyCartValidator : AbstractValidator<GetMyCartQuery>
{
    public GetMyCartValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid User ID format.");
    }
}