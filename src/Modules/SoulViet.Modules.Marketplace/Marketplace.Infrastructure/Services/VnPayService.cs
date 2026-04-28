using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Payments;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Services;

public class VnPayService : IVnPayService
{
    private readonly VnPayConfig _vnPayConfig;
    public VnPayService(IOptions<VnPayConfig> vnPayConfig)
    {
        _vnPayConfig = vnPayConfig.Value;
    }

    public string CreatePaymentUrl(MasterOrder masterOrder, HttpContext context)
    {
        var timeZoneId = OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh";
        var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
        var tick = DateTime.Now.Ticks.ToString();
        var pay = new VnPayLibrary();

        var tmnCode = Environment.GetEnvironmentVariable("VN_PAY_TMN_CODE")
                      ?? Environment.ExpandEnvironmentVariables(_vnPayConfig.TmnCode ?? string.Empty);
        var hashSecret = Environment.GetEnvironmentVariable("VN_PAY_HASH_SECRET")
                         ?? Environment.ExpandEnvironmentVariables(_vnPayConfig.HashSecret ?? string.Empty);
        var baseUrl = Environment.GetEnvironmentVariable("VN_PAY_BASE_URL")
                      ?? Environment.ExpandEnvironmentVariables(_vnPayConfig.BaseUrl ?? string.Empty);
        var returnUrl = Environment.GetEnvironmentVariable("VN_PAY_RETURN_URL")
                        ?? Environment.ExpandEnvironmentVariables(_vnPayConfig.ReturnUrl ?? string.Empty);

        pay.AddRequestData("vnp_Version", _vnPayConfig.Version);
        pay.AddRequestData("vnp_Command", _vnPayConfig.Command);
        pay.AddRequestData("vnp_TmnCode", tmnCode);

        pay.AddRequestData("vnp_Amount", ((long)(masterOrder.FinalPayableAmount * 100)).ToString());

        pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        pay.AddRequestData("vnp_CurrCode", _vnPayConfig.CurrencyCode);

        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        pay.AddRequestData("vnp_IpAddr", ipAddress);

        pay.AddRequestData("vnp_Locale", _vnPayConfig.Locale);
        pay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {masterOrder.Id}");
        pay.AddRequestData("vnp_OrderType", "other");
        pay.AddRequestData("vnp_ReturnUrl", returnUrl);

        pay.AddRequestData("vnp_TxnRef", $"{masterOrder.Id}_{tick}");
        pay.AddRequestData("vnp_ExpireDate", timeNow.AddDays(1).ToString("yyyyMMddHHmmss"));

        var paymentUrl = pay.CreateRequestUrl(baseUrl, hashSecret);

        return paymentUrl;
    }

    public string CreatePaymentUrl(decimal amount, string txnRef, string orderInfo, HttpContext context)
{
    var timeZoneId = OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh";
    var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
    var pay = new VnPayLibrary();

    var tmnCode = Environment.GetEnvironmentVariable("VN_PAY_TMN_CODE") ?? Environment.ExpandEnvironmentVariables(_vnPayConfig.TmnCode ?? string.Empty);
    var hashSecret = Environment.GetEnvironmentVariable("VN_PAY_HASH_SECRET") ?? Environment.ExpandEnvironmentVariables(_vnPayConfig.HashSecret ?? string.Empty);
    var baseUrl = Environment.GetEnvironmentVariable("VN_PAY_BASE_URL") ?? Environment.ExpandEnvironmentVariables(_vnPayConfig.BaseUrl ?? string.Empty);
    var returnUrl = Environment.GetEnvironmentVariable("VN_PAY_RETURN_URL") ?? Environment.ExpandEnvironmentVariables(_vnPayConfig.ReturnUrl ?? string.Empty);

    pay.AddRequestData("vnp_Version", _vnPayConfig.Version);
    pay.AddRequestData("vnp_Command", _vnPayConfig.Command);
    pay.AddRequestData("vnp_TmnCode", tmnCode);

    // Xử lý số tiền tùy chỉnh (nhân 100 theo format VNPay)
    pay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());

    pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
    pay.AddRequestData("vnp_CurrCode", _vnPayConfig.CurrencyCode);

    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
    pay.AddRequestData("vnp_IpAddr", ipAddress);

    pay.AddRequestData("vnp_Locale", _vnPayConfig.Locale);

    pay.AddRequestData("vnp_OrderInfo", orderInfo);
    pay.AddRequestData("vnp_OrderType", "other");
    pay.AddRequestData("vnp_ReturnUrl", returnUrl);

    pay.AddRequestData("vnp_TxnRef", txnRef);

    // Phòng chia tiền có thời hạn ngắn, nên link VNPay cũng chỉ nên sống khoảng 30 phút
    pay.AddRequestData("vnp_ExpireDate", timeNow.AddMinutes(30).ToString("yyyyMMddHHmmss"));

    return pay.CreateRequestUrl(baseUrl, hashSecret);
}
}