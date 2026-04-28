using MediatR;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.BillSplitting.Commands.ProcessSplitPaymentIpn;

public class ProcessSplitPaymentIpnCommand : IRequest<bool>
{
    public string TxnRef { get; set; } = string.Empty;
    public string ResponseCode { get; set; } = string.Empty;
}