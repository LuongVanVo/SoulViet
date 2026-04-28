using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.CancelOrder;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.CreateOrder;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.ProcessVnPayIpn;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.ScanTicket;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.UpdateOrderStatus;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetAdminMasterOrderDetail;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetAllOrdersForAdmin;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetMasterOrderDetail;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetOrderHistory;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetRepayUrl;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetShopOrders;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.PreviewOrder;
using SoulViet.Modules.Marketplace.Marketplace.Presentation.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace SoulViet.Modules.Marketplace.Marketplace.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/marketplace/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("preview")]
    [SwaggerOperation(Summary = "Preview order details",
        Description =
            "Previews the order details based on the selected cart items and applied vouchers before placing the order.")]
    public async Task<IActionResult> PreviewOrder([FromBody] PreviewOrderQuery query,
        CancellationToken cancellationToken)
    {
        var userId = User.GetCurrentUserId();
        query.UserId = userId;

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new order",
        Description = "Creates a new order based on the selected cart items and applied vouchers.")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        command.UserId = User.GetCurrentUserId();
        command.ReceiverEmail = User.GetCurrentUserEmail();

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("vnpay-ipn")]
    [SwaggerOperation(Summary = "VnPay IPN endpoint",
        Description = "Endpoint to receive Instant Payment Notifications (IPN) from VnPay after a payment is processed.")]
    public async Task<IActionResult> VnPayIpn()
    {
        var requestData = new Dictionary<string, string>();
        foreach (var (key, value) in Request.Query)
        {
            requestData.Add(key, value.ToString());
        }

        var command = new ProcessVnPayIpnCommand
        {
            RequestData = requestData
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("vnpay-return")]
    [SwaggerOperation(Summary = "VnPay return endpoint",
        Description =
            "Endpoint to handle the return URL from VnPay after a payment is processed. This can be used to display a success or failure message to the user based on the payment result.")]
    public IActionResult VnPayReturn()
    {
        var vnp_ResponseCode = Request.Query["vnp_ResponseCode"].ToString();
        var vnp_TxnRef = Request.Query["vnp_TxnRef"].ToString();
        var vnp_Amount = Request.Query["vnp_Amount"].ToString();
        var vnp_TransactionNo = Request.Query["vnp_TransactionNo"].ToString();
        var vnp_OrderInfo = Request.Query["vnp_OrderInfo"].ToString();
        var vnp_TransactionStatus = Request.Query["vnp_TransactionStatus"].ToString();

        if (vnp_TxnRef.StartsWith("SPLIT-"))
        {
            var parts = vnp_TxnRef.Split('-');
            var roomId = parts[1];

            return Redirect($"http://localhost:3000/split-payment/room/{roomId}?responseCode={vnp_ResponseCode}&transactionNo={vnp_TransactionNo}&orderInfo={Uri.EscapeDataString(vnp_OrderInfo)}&transactionStatus={vnp_TransactionStatus}");
        }

        var orderId = vnp_TxnRef.Split('_')[0];

        var amount = Convert.ToDecimal(vnp_Amount) / 100;

        var result = new
        {
            Success = vnp_ResponseCode == "00",
            Message = vnp_ResponseCode == "00" ? "Thanh toán thành công" : "Thanh toán thất bại hoặc bị hủy",
            OrderId = orderId,
            VnPayTransactionId = vnp_TransactionNo,
            Amount = amount,
            ResponseCode = vnp_ResponseCode,
            TransactionStatus = vnp_TransactionStatus,
            Description = vnp_OrderInfo,
            RedirectUrl = vnp_ResponseCode == "00"
                ? $"http://localhost:3000/checkout/success?orderId={orderId}"
                : $"http://localhost:3000/checkout/failed?orderId={orderId}&error={vnp_ResponseCode}"
        };
        return Ok(result);

        // if (vnp_ResponseCode == "00")
        // {
        //     return Redirect($"http://localhost:3000/checkout/success?orderId={orderId}");
        // }
        // return Redirect($"http://localhost:3000/checkout/failed?orderId={orderId}");
    }

    [HttpGet("history")]
    [SwaggerOperation(Summary = "Get order history",
        Description =
            "Retrieves the order history for the currently authenticated user, including details of past orders, their statuses, and associated information.")]
    public async Task<IActionResult> GetOrderHistory([FromQuery] GetOrderHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var userId = User.GetCurrentUserId();

        var result = await _mediator.Send(new GetOrderHistoryQuery
        {
            UserId = userId,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            PaymentStatus = query.PaymentStatus,
            PaymentMethod = query.PaymentMethod,
            FromDate = query.FromDate,
            ToDate = query.ToDate
        }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{masterOrderId}")]
    [SwaggerOperation(Summary = "Get order details",
        Description = "Retrieves the details of a specific order by its ID, including items, pricing, status, and any applied vouchers or discounts.")]
    public async Task<IActionResult> GetOrderDetails([FromRoute] Guid masterOrderId, CancellationToken cancellationToken)
    {
        var userId = User.GetCurrentUserId();

        var query = new GetMasterOrderDetailQuery()
        {
            MasterOrderId = masterOrderId,
            UserId = userId
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{masterOrderId}/cancel")]
    [SwaggerOperation(Summary = "Cancel an order",
        Description =
            "Allows the user to cancel an order that has not yet been processed or shipped. The cancellation will update the order status and trigger any necessary refund processes if payment has already been made.")]
    public async Task<IActionResult> CancelOrder([FromRoute] Guid masterOrderId)
    {
        var command = new CancelOrderCommand
        {
            MasterOrderId = masterOrderId,
            UserId = User.GetCurrentUserId()
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [Authorize(Roles = "LocalPartner")]
    [HttpGet("local-partner")]
    [SwaggerOperation(Summary = "Get local partner orders",
        Description =
            "Retrieves a list of orders associated with the local partner, allowing them to view and manage their orders.")]
    public async Task<IActionResult> GetShopOrders([FromQuery] GetShopOrdersQuery query,
        CancellationToken cancellationToken)
    {
        query.PartnerId = User.GetCurrentUserId();

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "LocalPartner")]
    [HttpPatch("local-partner/{orderId}/status")]
    [SwaggerOperation(Summary = "Update local partner order status",
        Description =
            "Allows the local partner to update the status of an order, such as marking it as processed, shipped, or delivered, to keep customers informed about the progress of their orders.")]
    public async Task<IActionResult> UpdateOrderStatus([FromRoute] Guid orderId,
        [FromBody] UpdateOrderStatusCommand command)
    {
        command.PartnerId = User.GetCurrentUserId();
        command.OrderId = orderId;

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    [SwaggerOperation(Summary = "Get all orders (admin)",
        Description =
            "Retrieves a list of all orders in the system for administrative purposes, allowing admins to monitor and manage orders across the entire marketplace.")]
    public async Task<IActionResult> GetAllOrders([FromQuery] GetAllOrdersForAdminQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/{masterOrderId}")]
    [SwaggerOperation(Summary = "Get order details (admin)",
        Description =
            "Retrieves the details of a specific order by its ID for administrative purposes, allowing admins to view and manage order details across the entire marketplace.")]
    public async Task<IActionResult> GetOrderDetailsForAdmin([FromRoute] Guid masterOrderId,
        CancellationToken cancellationToken)
    {
        var query = new GetAdminMasterOrderDetailQuery()
        {
            MasterOrderId = masterOrderId
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{masterOrderId}/repay-url")]
    [SwaggerOperation(Summary = "Get repay URL for pending orders",
        Description =
            "Generates a new payment URL for orders that have a pending payment status, allowing users to complete their payment if the initial payment attempt was unsuccessful or if they need to retry the payment process.")]
    public async Task<IActionResult> GetRepayUrl([FromRoute] Guid masterOrderId)
    {
        var query = new GetRepayUrlQuery
        {
            UserId = User.GetCurrentUserId(),
            MasterOrderId = masterOrderId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = ("LocalPartner"))]
    [HttpPost("scan")]
    [SwaggerOperation(Summary = "Scan QR code to check-in service orders of a local partner",
        Description =
            "Allows local partners to scan a QR code associated with a service order to check-in and mark the service as used, providing a seamless way to manage service orders and track their usage in real-time.")]
    public async Task<IActionResult> ScanTicket([FromBody] ScanTicketCommand command)
    {
        var partnerId = User.GetCurrentUserId();
        command.PartnerId = partnerId;

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}