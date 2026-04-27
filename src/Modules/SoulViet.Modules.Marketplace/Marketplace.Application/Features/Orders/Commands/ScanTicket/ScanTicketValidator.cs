using FluentValidation;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.ScanTicket;

public class ScanTicketValidator : AbstractValidator<ScanTicketCommand>
{
    public ScanTicketValidator()
    {
        RuleFor(x => x.PartnerId)
            .NotEmpty().WithMessage("PartnerId is required.")
            .Must(partnerId => Guid.TryParse(partnerId.ToString(), out _)).WithMessage("PartnerId must be a valid GUID.");
    }
}