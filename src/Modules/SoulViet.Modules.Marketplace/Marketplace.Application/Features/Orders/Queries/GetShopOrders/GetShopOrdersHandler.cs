using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetShopOrders;

public class GetShopOrdersHandler : IRequestHandler<GetShopOrdersQuery, PaginatedList<ShopOrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    public GetShopOrdersHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ShopOrderDto>> Handle(GetShopOrdersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _orderRepository.GetShopOrdersWithPaginationAsync(
            request.PartnerId,
            request.PageNumber,
            request.PageSize,
            request.Status,
            request.PaymentStatus,
            request.PaymentMethod,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var dtos = _mapper.Map<List<ShopOrderDto>>(items);

        return new PaginatedList<ShopOrderDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}