using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetMyTickets;

public class GetMyTicketsHandler : IRequestHandler<GetMyTicketsQuery, PaginatedList<MyTicketDto>>
{
    private readonly IOrderItemRepository _orderItemRepository;
    public GetMyTicketsHandler(IOrderItemRepository orderItemRepository)
    {
        _orderItemRepository = orderItemRepository;
    }

    public async Task<PaginatedList<MyTicketDto>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _orderItemRepository.GetUserTicketsWithPaginationAsync(
            request.UserId,
            request.Status,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return new PaginatedList<MyTicketDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}