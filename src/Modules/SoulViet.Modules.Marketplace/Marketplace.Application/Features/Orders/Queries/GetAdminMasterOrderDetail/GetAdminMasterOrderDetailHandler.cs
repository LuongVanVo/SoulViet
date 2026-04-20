using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetAdminMasterOrderDetail;

public class GetAdminMasterOrderDetailHandler : IRequestHandler<GetAdminMasterOrderDetailQuery, AdminMasterOrderDetailDto>
{
    private readonly IMasterOrderRepository _masterOrderRepository;
    private readonly IMapper _mapper;

    public GetAdminMasterOrderDetailHandler(IMasterOrderRepository masterOrderRepository, IMapper mapper)
    {
        _masterOrderRepository = masterOrderRepository;
        _mapper = mapper;
    }

    public async Task<AdminMasterOrderDetailDto> Handle(GetAdminMasterOrderDetailQuery request, CancellationToken cancellationToken)
    {
        var order = await _masterOrderRepository.GetByIdWithDetailsAsync(request.MasterOrderId, cancellationToken);
        if (order == null)
        {
            throw new KeyNotFoundException("Master order not found.");
        }

        return _mapper.Map<AdminMasterOrderDetailDto>(order);
    }
}