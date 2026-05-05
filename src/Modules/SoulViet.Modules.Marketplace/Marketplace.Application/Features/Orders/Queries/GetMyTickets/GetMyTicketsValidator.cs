using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetMyTickets;

public class GetMyTicketsValidator : AbstractValidator<GetMyTicketsQuery>
{
     public GetMyTicketsValidator()
     {
          RuleFor(x => x.UserId)
               .NotEmpty().WithMessage("User ID is required.")
               .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid user ID format.");

          RuleFor(x => x.Status)
               .IsInEnum().WithMessage("Invalid status value.");

          RuleFor(x => x.PageNumber)
               .GreaterThan(0).WithMessage("Page number must be greater than 0.");

          RuleFor(x => x.PageSize)
               .GreaterThan(0).WithMessage("Page size must be greater than 0.")
               .LessThanOrEqualTo(100).WithMessage("Page size must be 100 or less.");
     }
}