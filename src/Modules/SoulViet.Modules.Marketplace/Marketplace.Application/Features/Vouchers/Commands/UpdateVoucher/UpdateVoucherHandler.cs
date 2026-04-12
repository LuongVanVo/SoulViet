using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.UpdateVoucher;

public class UpdateVoucherHandler : IRequestHandler<UpdateVoucherCommand, VoucherResponse>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateVoucherHandler(IVoucherRepository voucherRepository, IUnitOfWork unitOfWork)
    {
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VoucherResponse> Handle(UpdateVoucherCommand request, CancellationToken cancellationToken)
    {
        var voucher = await _voucherRepository.GetByIdAsync(request.Id, cancellationToken);

        if (voucher == null)
            throw new NotFoundException($"Voucher with ID {request.Id} not found.");

        if (!request.IsAdmin)
        {
            if (voucher.PartnerId != request.PartnerId)
                throw new ForbiddenException("You do not have permission to update this voucher.");
        }

        if (request.EndDate.HasValue)
            voucher.EndDate = request.EndDate.Value;

        if (request.UsageLimit.HasValue)
            voucher.UsageLimit = request.UsageLimit.Value;

        if (request.MinOrderAmount.HasValue)
            voucher.MinOrderAmount = request.MinOrderAmount.Value;

        if (request.MaxDiscountAmount.HasValue)
            voucher.MaxDiscountAmount = request.MaxDiscountAmount.Value;

        _voucherRepository.Update(voucher);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new VoucherResponse
        {
            Success = true,
            Message = "Voucher partially updated successfully.",
            Id = voucher.Id,
            Code = voucher.Code
        };
    }
}