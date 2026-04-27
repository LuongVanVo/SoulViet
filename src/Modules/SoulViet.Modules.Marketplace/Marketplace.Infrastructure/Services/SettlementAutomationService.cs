using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Commands.GeneratePayoutBatches;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Services;

public class SettlementAutomationService : ISettlementAutomationService
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    public SettlementAutomationService(IMediator mediator, IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    public async Task ProcessAutoSettlementAsync()
    {
        var command = new GeneratePayoutBatchesCommand
        {
            EndDate = DateTime.UtcNow
        };

        await _mediator.Send(command);

        // Sau này có thể thêm tích hợp chuyển tiền tự động qua ngân hàng.

    }
}