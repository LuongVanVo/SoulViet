using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Commands.MarkAsPaid;

public class MarkAsPaidHandler : IRequestHandler<MarkAsPaidCommand, bool>
{
    private readonly IPayoutBatchRepository _payoutBatchRepository;
    private readonly IUnitOfWork _unitOfWork;
    public MarkAsPaidHandler(IPayoutBatchRepository payoutBatchRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _payoutBatchRepository = payoutBatchRepository;
    }

    public async Task<bool> Handle(MarkAsPaidCommand request, CancellationToken cancellationToken)
    {
        var batch = await _payoutBatchRepository.GetByIdAsync(request.BatchId, cancellationToken);
        if (batch == null)
            throw new NotFoundException("Payout batch not found");

        if (batch.Status == SettlementStatus.Completed)
            throw new BadRequestException("Batch is already marked as paid");

        if (string.IsNullOrWhiteSpace(request.TransactionReference))
            throw new BadRequestException("Please provide a valid transaction reference for the payout");

        batch.Status = SettlementStatus.Completed;
        batch.TransactionReference = request.TransactionReference;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notification for partner is money has been transferred can be implemented here (e.g., via an event or direct call to a notification service)

        return true;
    }
}