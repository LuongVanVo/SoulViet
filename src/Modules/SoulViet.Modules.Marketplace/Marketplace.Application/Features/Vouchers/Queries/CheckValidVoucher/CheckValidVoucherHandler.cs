using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.CheckValidVoucher;

public class CheckValidVoucherHandler : IRequestHandler<CheckValidVoucherQuery, VoucherDto>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IMapper _mapper;
    public CheckValidVoucherHandler(IVoucherRepository voucherRepository, IMapper mapper)
    {
        _voucherRepository = voucherRepository;
        _mapper = mapper;
    }

    public async Task<VoucherDto> Handle(CheckValidVoucherQuery request, CancellationToken cancellationToken)
    {
        var voucher = await _voucherRepository.GetValidVoucherAsync(request.Code, request.PartnerId, cancellationToken);
        if (voucher == null)
            throw new BadRequestException($"Voucher code '{request.Code}' is invalid.");

        if (request.CurrentOrderAmount < voucher.MinOrderAmount)
        {
            throw new BadRequestException($"This voucher requires a minimum order amount of {voucher.MinOrderAmount}. Your current amount is {request.CurrentOrderAmount}.");
        }

        return _mapper.Map<VoucherDto>(voucher);
    }
}