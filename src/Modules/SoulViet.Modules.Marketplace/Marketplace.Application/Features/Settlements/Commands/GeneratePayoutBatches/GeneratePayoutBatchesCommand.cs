using MediatR;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Commands.GeneratePayoutBatches;

public class GeneratePayoutBatchesCommand : IRequest<bool>
{
    public DateTime EndDate { get; set; }
}