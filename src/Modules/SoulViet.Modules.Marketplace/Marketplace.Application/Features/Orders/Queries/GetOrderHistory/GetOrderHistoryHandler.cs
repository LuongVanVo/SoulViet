using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetOrderHistory;

public class GetOrderHistoryHandler : IRequestHandler<GetOrderHistoryQuery, PaginatedList<OrderHistoryItemDto>>
{
    private readonly IMasterOrderRepository _masterOrderRepository;
    private readonly IMapper _mapper;
    public GetOrderHistoryHandler(IMasterOrderRepository masterOrderRepository, IMapper mapper)
    {
        _masterOrderRepository = masterOrderRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<OrderHistoryItemDto>> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _masterOrderRepository.GetByUserIdWithPaginationAsync(
            request.UserId,
            request.PageNumber,
            request.PageSize,
            request.PaymentStatus,
            request.PaymentMethod,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        var dtos = _mapper.Map<List<OrderHistoryItemDto>>(items);

        return new PaginatedList<OrderHistoryItemDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}