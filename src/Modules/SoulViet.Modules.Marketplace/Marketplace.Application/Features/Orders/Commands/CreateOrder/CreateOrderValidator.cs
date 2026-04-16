using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid user ID format.");

        RuleFor(x => x.SelectedCartItemIds)
            .NotEmpty().WithMessage("At least one cart item must be selected.")
            .Must(ids => ids.All(id => Guid.TryParse(id.ToString(), out _)))
            .WithMessage("All cart item IDs must be in a valid format.");
    }
}