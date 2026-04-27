using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Commands.MarkAsPaid;

public class MarkAsPaidValidator : AbstractValidator<MarkAsPaidCommand>
{
    public MarkAsPaidValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty().WithMessage("Batch ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Batch ID must be a valid GUID.");
    }
}