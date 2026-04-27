using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Helpers;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.ScanTicket;

public class ScanTicketHandler : IRequestHandler<ScanTicketCommand, ScanTicketResponse>
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ScanTicketHandler(IOrderItemRepository orderItemRepository, IUnitOfWork unitOfWork)
    {
        _orderItemRepository = orderItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ScanTicketResponse> Handle(ScanTicketCommand request, CancellationToken cancellationToken)
    {
        // Using helper to decode TicketCode
        if (!TicketSecurityHelper.VerifyTicketCode(request.TicketCode, out Guid orderItemId))
        {
            throw new BadRequestException("Ticket code is invalid or has been counterfeited!");
        }

        // Find ticket
        var orderItem = await _orderItemRepository.GetTicketByIdWithOrderAsync(orderItemId, cancellationToken);
        if (orderItem == null)
            throw new NotFoundException("Ticket not found!");

        // Access control shop
        if (orderItem.Order.PartnerId != request.PartnerId)
            throw new ForbiddenException("You do not have permission to access this ticket!");

        // Check if ticket is already used
        if (orderItem.IsTicketUsed)
            throw new BadRequestException($"This ticket has already been used at {orderItem.TicketUsedDate.Value.ToString("yyyy-MM-dd HH:mm:ss")}");

        // Mark ticket as used
        orderItem.IsTicketUsed = true;
        orderItem.TicketUsedDate = DateTime.UtcNow;
        _orderItemRepository.Update(orderItem);

        // Check auto-complete order
        var allItemsInVendorOrder =
            await _orderItemRepository.GetItemsByOrderIdAsync(orderItem.OrderId, cancellationToken);

        var hasPhysicalGoods = allItemsInVendorOrder.Any(x => x.ProductTypeSnapshot == ProductType.PhysicalGoods);

        var allServicesUsed = allItemsInVendorOrder
            .Where(x => x.ProductTypeSnapshot != ProductType.PhysicalGoods)
            .All(x => x.IsTicketUsed);

        if (!hasPhysicalGoods && allServicesUsed)
        {
            orderItem.Order.Status = OrderStatus.Delivered;
        }

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ScanTicketResponse
        {
            Success = true,
            Message = "Ticket scanned successfully!",
            ProductName = orderItem.ProductNameSnapshot,
            Quantity = orderItem.Quantity,
            UsedAt = orderItem.TicketUsedDate.Value
        };
    }
}