using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.")
            .Must(id => id != Guid.Empty).WithMessage("OrderId must be a valid GUID.");

        RuleFor(x => x.PartnerId)
            .NotEmpty().WithMessage("PartnerId is required.")
            .Must(id => id != Guid.Empty).WithMessage("PartnerId must be a valid GUID.");

        RuleFor(x => x.NewStatus)
            .NotEmpty().WithMessage("NewStatus is required.")
            .IsInEnum().WithMessage("NewStatus must be a valid OrderStatus.");
    }
}