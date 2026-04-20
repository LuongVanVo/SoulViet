using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetAllOrdersForAdmin;

public class GetAllOrdersForAdminHandler : IRequestHandler<GetAllOrdersForAdminQuery, PaginatedList<AdminMasterOrderDto>>
{
    private readonly IMasterOrderRepository _masterOrderRepository;
    private readonly IMapper _mapper;
    public GetAllOrdersForAdminHandler(IMasterOrderRepository masterOrderRepository, IMapper mapper)
    {
        _masterOrderRepository = masterOrderRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<AdminMasterOrderDto>> Handle(GetAllOrdersForAdminQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _masterOrderRepository.GetMasterOrdersWithPaginationForAdminAsync(
            request.PageNumber,
            request.PageSize,
            request.PaymentStatus,
            request.PaymentMethod,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var dtos = _mapper.Map<List<AdminMasterOrderDto>>(items);

        return new PaginatedList<AdminMasterOrderDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}