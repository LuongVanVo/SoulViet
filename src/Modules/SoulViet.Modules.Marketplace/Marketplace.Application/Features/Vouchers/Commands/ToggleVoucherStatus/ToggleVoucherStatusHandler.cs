using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.ToggleVoucherStatus;

public class ToggleVoucherStatusHandler : IRequestHandler<ToggleVoucherStatusCommand, VoucherResponse>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ToggleVoucherStatusHandler(IVoucherRepository voucherRepository, IUnitOfWork unitOfWork)
    {
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VoucherResponse> Handle(ToggleVoucherStatusCommand request, CancellationToken cancellationToken)
    {
        var voucher = await _voucherRepository.GetByIdAsync(request.Id, cancellationToken);
        if (voucher == null)
            throw new NotFoundException($"Voucher with ID '{request.Id}' not found.");

        if (!request.IsAdmin)
        {
            if (voucher.PartnerId != request.PartnerId)
                throw new ForbiddenException("You do not have permission to change the status of this voucher.");
        }

        voucher.IsActive = !voucher.IsActive;

        _voucherRepository.Update(voucher);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var statusText  = voucher.IsActive ? "activated" : "deactivated";
        return new VoucherResponse
        {
            Success = true,
            Message = $"Voucher has been {statusText} successfully.",
            Id = voucher.Id,
            Code = voucher.Code
        };
    }
}