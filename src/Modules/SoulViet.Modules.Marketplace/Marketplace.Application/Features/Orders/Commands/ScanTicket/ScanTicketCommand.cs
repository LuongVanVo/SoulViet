using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Results;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.ScanTicket;

public class ScanTicketCommand : IRequest<ScanTicketResponse>
{
    public string TicketCode { get; set; } = string.Empty;
    public Guid PartnerId { get; set; }
}