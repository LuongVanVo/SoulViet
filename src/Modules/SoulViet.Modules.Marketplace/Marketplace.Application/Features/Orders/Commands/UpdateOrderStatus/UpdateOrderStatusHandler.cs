using MediatR;
using Microsoft.Extensions.Logging;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrderStatusHandler> _logger;
    public UpdateOrderStatusHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, ILogger<UpdateOrderStatusHandler> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null || order.PartnerId != request.PartnerId)
            throw new NotFoundException("Order not found or access denied.");

        // Logic state machine
        if (order.Status == OrderStatus.Cancelled)
            throw new BadRequestException("Cannot update status of a cancelled order.");

        if (order.Status == OrderStatus.Delivered)
            throw new BadRequestException("Cannot update status of a delivered order.");

        bool isValidTransition = (order.Status == OrderStatus.Pending && (request.NewStatus == OrderStatus.Processing || request.NewStatus == OrderStatus.Cancelled)) ||
                                 (order.Status == OrderStatus.Processing && request.NewStatus == OrderStatus.Shipped) ||
                                 (order.Status == OrderStatus.Shipped && request.NewStatus == OrderStatus.Delivered);

        if (!isValidTransition)
            throw new BadRequestException($"Invalid status transition from {order.Status} to {request.NewStatus}.");

        // Update
        order.Status = request.NewStatus;

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} status updated to {NewStatus} by partner {PartnerId}", request.OrderId, request.NewStatus, request.PartnerId);

        return true;
    }
}