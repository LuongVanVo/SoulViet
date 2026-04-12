using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetAvailableVouchers;

public class GetAvailableVouchersHandler : IRequestHandler<GetAvailableVouchersQuery, List<VoucherDto>>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IMapper _mapper;
    public GetAvailableVouchersHandler(IVoucherRepository voucherRepository, IMapper mapper)
    {
        _voucherRepository = voucherRepository;
        _mapper = mapper;
    }

    public async Task<List<VoucherDto>> Handle(GetAvailableVouchersQuery request, CancellationToken cancellationToken)
    {
        var availableVouchers =
            await _voucherRepository.GetAvailableVouchersAsync(request.PartnerId, request.CurrentOrderAmount, cancellationToken);

        return _mapper.Map<List<VoucherDto>>(availableVouchers);
    }
}