using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetAdminMasterOrderDetail;

public class GetAdminMasterOrderDetailQuery : IRequest<AdminMasterOrderDetailDto>
{
    public Guid MasterOrderId { get; set; }
}