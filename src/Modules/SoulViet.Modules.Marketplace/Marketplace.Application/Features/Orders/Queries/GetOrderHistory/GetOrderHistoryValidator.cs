using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetOrderHistory;

public class GetOrderHistoryValidator : AbstractValidator<GetOrderHistoryQuery>
{
    public GetOrderHistoryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(id => id != Guid.Empty).WithMessage("UserId must be a valid GUID.");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("PageSize must be less than or equal to 100.");
    }
}