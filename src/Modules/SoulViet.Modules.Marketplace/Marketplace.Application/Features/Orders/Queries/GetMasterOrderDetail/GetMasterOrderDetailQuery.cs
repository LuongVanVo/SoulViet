using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetMasterOrderDetail;

public class GetMasterOrderDetailQuery : IRequest<MasterOrderDetailDto>
{
    public Guid MasterOrderId { get; set; }
    public Guid UserId { get; set; }
}