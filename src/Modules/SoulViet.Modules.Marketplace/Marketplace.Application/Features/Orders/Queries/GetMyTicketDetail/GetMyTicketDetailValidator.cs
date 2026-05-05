using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetMyTicketDetail;

public class GetMyTicketDetailValidator : AbstractValidator<GetMyTicketDetailQuery>
{
    public GetMyTicketDetailValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(id => id != Guid.Empty).WithMessage("UserId must be a valid GUID.");

        RuleFor(x => x.OrderItemId)
            .NotEmpty().WithMessage("OrderItemId is required.")
            .Must(id => id != Guid.Empty).WithMessage("OrderItemId must be a valid GUID.");
    }
}