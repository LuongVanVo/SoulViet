using MediatR;
using Microsoft.AspNetCore.Http;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetRepayUrl;

public class GetRepayUrlHandler : IRequestHandler<GetRepayUrlQuery, string>
{
    private readonly IMasterOrderRepository _masterOrderRepository;
    private readonly IVnPayService _vnPayService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetRepayUrlHandler(IMasterOrderRepository masterOrderRepository, IVnPayService vnPayService, IHttpContextAccessor httpContextAccessor)
    {
        _masterOrderRepository = masterOrderRepository;
        _vnPayService = vnPayService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(GetRepayUrlQuery request, CancellationToken cancellationToken)
    {
        var order = await _masterOrderRepository.GetByIdAsync(request.MasterOrderId, cancellationToken);

        if (order == null || order.UserId != request.UserId)
            throw new BadRequestException("Order not found or does not belong to the user.");

        if (order.PaymentStatus != PaymentStatus.Pending)
            throw new BadRequestException("Only orders with pending payment can be repaid.");

        if (order.PaymentMethod != PaymentMethod.VnPay)
            throw new BadRequestException("This order does not use method payment online, cannot generate repay URL.");

        // Generate new link
        var context = _httpContextAccessor.HttpContext;
        if (context == null) throw new Exception("HttpContext is null.");

        var paymentUrl = _vnPayService.CreatePaymentUrl(order, context);

        return paymentUrl;
    }
}