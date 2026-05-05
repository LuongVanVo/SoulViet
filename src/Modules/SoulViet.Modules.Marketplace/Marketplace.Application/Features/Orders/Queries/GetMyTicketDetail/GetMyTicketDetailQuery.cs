using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetMyTicketDetail;

public class GetMyTicketDetailQuery : IRequest<MyTicketDetailDto>
{
    public Guid UserId { get; set; }
    public Guid OrderItemId { get; set; }
}