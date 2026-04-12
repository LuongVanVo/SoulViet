using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.CreateVoucher;

public class CreateVoucherHandler : IRequestHandler<CreateVoucherCommand, VoucherResponse>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public CreateVoucherHandler(IVoucherRepository voucherRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<VoucherResponse> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
    {
        var voucherCode = request.Code.Trim().ToUpper();

        var existingVoucher = await _voucherRepository.GetByCodeAsync(voucherCode, cancellationToken);
        if (existingVoucher != null)
            throw new BadRequestException($"Voucher code '{voucherCode}' already exists.");

        var voucher = _mapper.Map<Voucher>(request);

        voucher.Id = Guid.NewGuid();
        voucher.Code = voucherCode;
        voucher.UsedCount = 0;
        voucher.IsActive = true;

        await _voucherRepository.AddAsync(voucher, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new VoucherResponse
        {
            Success = true,
            Message = "Voucher created successfully.",
            Id = voucher.Id,
            Code = voucher.Code
        };
    }
}