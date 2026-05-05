using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Common;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetMyTickets;

public class GetMyTicketsQuery : IRequest<PaginatedList<MyTicketDto>>
{
    public Guid UserId { get; set; }
    public TicketStatusFilter Status { get; set; } = TicketStatusFilter.All;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}