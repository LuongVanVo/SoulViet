using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.GetMasterOrderDetail;

public class GetMasterOrderDetailHandler : IRequestHandler<GetMasterOrderDetailQuery, MasterOrderDetailDto>
{
    private readonly IMasterOrderRepository _masterOrderRepository;
    private readonly IMapper _mapper;
    public GetMasterOrderDetailHandler(IMasterOrderRepository masterOrderRepository, IMapper mapper)
    {
        _masterOrderRepository = masterOrderRepository;
        _mapper = mapper;
    }

    public async Task<MasterOrderDetailDto> Handle(GetMasterOrderDetailQuery request, CancellationToken cancellationToken)
    {
        var order = await _masterOrderRepository.GetByIdWithDetailsAsync(request.MasterOrderId, cancellationToken);
        if (order == null || order.UserId != request.UserId)
        {
            throw new NotFoundException("Master order not found or access denied.");
        }

        return _mapper.Map<MasterOrderDetailDto>(order);
    }
}