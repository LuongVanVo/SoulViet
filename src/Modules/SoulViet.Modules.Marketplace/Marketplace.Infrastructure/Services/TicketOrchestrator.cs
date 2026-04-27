using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Helpers;
using SoulViet.Shared.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Services;

public class TicketOrchestrator : ITicketOrchestrator
{
    private readonly IMasterOrderRepository _masterOrderRepository;
    private readonly ITicketService _ticketService;
    private readonly IUnitOfWork _unitOfWork;
    public TicketOrchestrator(IMasterOrderRepository masterOrderRepository, ITicketService ticketService, IUnitOfWork unitOfWork)
    {
        _masterOrderRepository = masterOrderRepository;
        _ticketService = ticketService;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TicketEmailInfo>> ProcessTicketsForOrderAsync(Guid masterOrderId, CancellationToken cancellationToken = default)
    {
        var masterOrder = await _masterOrderRepository.GetByIdWithDetailsAsync(masterOrderId, cancellationToken);
        if (masterOrder == null)
            throw new NotFoundException("Master order not found");

        var generatedTickets = new List<TicketEmailInfo>();

        foreach (var vendorOrder in masterOrder.VendorOrders)
        {
            foreach (var item in vendorOrder.OrderItems)
            {
                if (string.IsNullOrEmpty(item.TicketCode) && item.ProductTypeSnapshot != ProductType.PhysicalGoods)
                {
                    var ticketCode = TicketSecurityHelper.GenerateTicketCode(item.Id, vendorOrder.PartnerId);
                    var qrUrl = await _ticketService.GenerateAndUploadQrCodeAsync(ticketCode);

                    item.TicketCode = ticketCode;
                    item.TicketQRUrl = qrUrl;

                    generatedTickets.Add(new TicketEmailInfo
                    {
                        ProductName = item.ProductNameSnapshot,
                        TicketCode = ticketCode,
                        QrUrl = qrUrl,
                        Quantity = item.Quantity
                    });
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return generatedTickets;
    }
}