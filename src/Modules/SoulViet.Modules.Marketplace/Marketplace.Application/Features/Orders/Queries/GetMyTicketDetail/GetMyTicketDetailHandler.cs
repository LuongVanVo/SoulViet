using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetMyTicketDetail;

public class GetMyTicketDetailHandler : IRequestHandler<GetMyTicketDetailQuery, MyTicketDetailDto>
{
    private readonly IOrderItemRepository _orderItemRepository;
    public GetMyTicketDetailHandler(IOrderItemRepository orderItemRepository)
    {
        _orderItemRepository = orderItemRepository;
    }

    public async Task<MyTicketDetailDto> Handle(GetMyTicketDetailQuery request, CancellationToken cancellationToken)
    {
        var detail = await _orderItemRepository.GetUserTicketDetailAsync(request.UserId, request.OrderItemId, cancellationToken);
        if (detail == null)
            throw new NotFoundException("Ticket not found or does not belong to the user.");

        return detail;
    }
}