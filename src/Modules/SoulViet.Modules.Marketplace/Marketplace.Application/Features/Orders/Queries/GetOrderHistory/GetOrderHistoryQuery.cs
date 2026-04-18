using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetOrderHistory;

public class GetOrderHistoryQuery : IRequest<PaginatedList<OrderHistoryItemDto>>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public PaymentStatus? PaymentStatus { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}