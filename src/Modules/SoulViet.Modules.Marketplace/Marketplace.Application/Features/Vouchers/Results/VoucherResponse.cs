namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Results;

public class VoucherResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public Guid? Id { get; set; }
    public string Code { get; set; } = string.Empty;
}