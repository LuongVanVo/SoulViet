using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.CreateOrder;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.ProcessVnPayIpn;
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
}