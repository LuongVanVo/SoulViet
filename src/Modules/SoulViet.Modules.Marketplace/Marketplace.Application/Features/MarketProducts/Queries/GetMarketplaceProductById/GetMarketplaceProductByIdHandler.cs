using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetMarketplaceProductById;

public class GetMarketplaceProductByIdHandler : IRequestHandler<GetMarketplaceProductByIdQuery, MarketplaceProductDto>
{
    private readonly IMarketplaceProductRepository _marketplaceProductRepository;
    private readonly IMapper _mapper;
    public GetMarketplaceProductByIdHandler(IMarketplaceProductRepository marketplaceProductRepository, IMapper mapper)
    {
        _marketplaceProductRepository = marketplaceProductRepository;
        _mapper = mapper;
    }

    public async Task<MarketplaceProductDto> Handle(GetMarketplaceProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _marketplaceProductRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new NotFoundException("Product not found");

        return _mapper.Map<MarketplaceProductDto>(product);
    }
}