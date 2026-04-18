using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Results;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.ProcessVnPayIpn;

public class ProcessVnPayIpnCommand : IRequest<VnPayIpnResponse>
{
    public Dictionary<string, string> RequestData { get; set; } = new();
}