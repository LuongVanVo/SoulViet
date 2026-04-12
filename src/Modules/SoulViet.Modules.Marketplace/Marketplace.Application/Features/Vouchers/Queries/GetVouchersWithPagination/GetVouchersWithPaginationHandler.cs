using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetVouchersWithPagination;

public class GetVouchersWithPaginationHandler : IRequestHandler<GetVouchersWithPaginationQuery, PaginatedList<VoucherDto>>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IMapper _mapper;
    public GetVouchersWithPaginationHandler(IVoucherRepository voucherRepository, IMapper mapper)
    {
        _mapper = mapper;
        _voucherRepository = voucherRepository;
    }

    public async Task<PaginatedList<VoucherDto>> Handle(GetVouchersWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _voucherRepository.GetVouchersWithPaginationAsync(
            partnerId: request.PartnerId,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            searchTerm: request.SearchTerm,
            isActive: request.IsActive,
            cancellationToken: cancellationToken
        );

        var dtos = _mapper.Map<List<VoucherDto>>(items);

        return new PaginatedList<VoucherDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}