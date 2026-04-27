using MediatR;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Commands.MarkAsPaid;

public class MarkAsPaidCommand : IRequest<bool>
{
    public Guid BatchId { get; set; }
    public string? TransactionReference { get; set; }
}