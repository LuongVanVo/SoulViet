using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetVoucherById;

public class GetVoucherByIdHandler : IRequestHandler<GetVoucherByIdQuery, VoucherDto>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IMapper _mapper;
    public GetVoucherByIdHandler(IVoucherRepository voucherRepository, IMapper mapper)
    {
        _voucherRepository = voucherRepository;
        _mapper = mapper;
    }

    public async Task<VoucherDto> Handle(GetVoucherByIdQuery request, CancellationToken cancellationToken)
    {
        var voucher = await _voucherRepository.GetByIdAsync(request.Id, cancellationToken);
        if (voucher == null)
            throw new NotFoundException($"Voucher with ID '{request.Id}' not found.");

        if (!request.IsAdmin)
        {
            if (voucher.PartnerId != request.PartnerId)
                throw new ForbiddenException("You do not have permission to view this voucher.");
        }

        return _mapper.Map<VoucherDto>(voucher);
    }
}