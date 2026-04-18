using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Payments;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.ProcessVnPayIpn;

public class ProcessVnPayIpnHandler : IRequestHandler<ProcessVnPayIpnCommand, VnPayIpnResponse>
{
    private readonly IMasterOrderRepository _masterOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly VnPayConfig _vnPayConfig;
    private readonly ILogger<ProcessVnPayIpnHandler> _logger;
    public ProcessVnPayIpnHandler(IMasterOrderRepository masterOrderRepository, IUnitOfWork unitOfWork, IOptions<VnPayConfig> vnPayConfig, ILogger<ProcessVnPayIpnHandler> logger)
    {
        _masterOrderRepository = masterOrderRepository;
        _unitOfWork = unitOfWork;
        _vnPayConfig = vnPayConfig.Value;
        _logger = logger;
    }

    public async Task<VnPayIpnResponse> Handle(ProcessVnPayIpnCommand request, CancellationToken cancellationToken)
    {
        var pay = new VnPayLibrary();

        foreach (var (key, value) in request.RequestData)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                pay.AddResponseData(key, value);
            }
        }

        var vnp_orderId = pay.GetResponseData("vnp_TxnRef");
        var vnp_TransactionId = pay.GetResponseData("vnp_TransactionNo");
        var vnp_SecureHash = request.RequestData.GetValueOrDefault("vnp_SecureHash");
        var vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
        var vnp_Amount = pay.GetResponseData("vnp_Amount");
        var vnp_PayDate = pay.GetResponseData("vnp_PayDate");

        // Validate signature
        bool checkSignature = pay.ValidateSignature(vnp_SecureHash, _vnPayConfig.HashSecret);
        if (!checkSignature)
        {
            _logger.LogWarning("VNPay IPN: Invalid signature for OrderId {OrderId}", vnp_orderId);
            return new VnPayIpnResponse { RspCode = "97", Message = "Invalid signature" };
        }

        try
        {
            var realOrderId = Guid.Parse(vnp_orderId.Split('_')[0]);
            var masterOrder = await _masterOrderRepository.GetByIdWithDetailsAsync(realOrderId, cancellationToken);

            if (masterOrder == null)
            {
                return new VnPayIpnResponse { RspCode = "01", Message = "Order not found" };
            }

            long vnpAmount = Convert.ToInt64(vnp_Amount) / 100;
            if (masterOrder.GrandTotal != vnpAmount)
            {
                return new VnPayIpnResponse { RspCode = "04", Message = "Invalid amount" };
            }

            if (masterOrder.PaymentStatus != PaymentStatus.Pending)
            {
                return new VnPayIpnResponse { RspCode = "02", Message = "Order already confirmed" };
            }

            // Update database
            DateTime? paymentDate = null;
            if (DateTime.TryParseExact(vnp_PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                var vnTimeZoneId = OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh";
                var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneId);

                paymentDate = TimeZoneInfo.ConvertTimeToUtc(parsedDate, vnTimeZone);
            }

            if (vnp_ResponseCode == "00")
            {
                // Payment successful
                masterOrder.PaymentStatus = PaymentStatus.Success;
                masterOrder.TransactionId = vnp_TransactionId;
                masterOrder.PaymentDate = paymentDate ?? DateTime.UtcNow;

                // Update order status of vendor orders
                foreach (var vendorOrder in masterOrder.VendorOrders)
                {
                    vendorOrder.Status = OrderStatus.Processing;
                }
                _logger.LogInformation("VNPay IPN: Successfully processed Order {OrderId}", realOrderId);
            }
            else
            {
                _logger.LogInformation("VNPay IPN: Payment attempt failed or cancelled for Order {OrderId}. VNPay Code: {Code}. Keeping status as Pending.", realOrderId, vnp_ResponseCode);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new VnPayIpnResponse
            {
                RspCode = "00",
                Message = "Confirm Success"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "VNPay IPN: Unknown error during processing");
            return new VnPayIpnResponse { RspCode = "99", Message = "Unknown error" };
        }
    }
}