using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetMyMarketplaceProducts;

public class GetMyMarketplaceProductsHandler : IRequestHandler<GetMyMarketplaceProductsQuery, PaginatedList<MarketplaceProductDto>>
{
    private readonly IMarketplaceProductRepository _marketplaceProductRepository;
    private readonly IMapper _mapper;
    public GetMyMarketplaceProductsHandler(IMarketplaceProductRepository marketplaceProductRepository, IMapper mapper)
    {
        _marketplaceProductRepository = marketplaceProductRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<MarketplaceProductDto>> Handle(GetMyMarketplaceProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _marketplaceProductRepository.GetPagedByPartnerIdAsync(
            request.PartnerId,
            request.SearchTerm,
            request.IsActive,
            request.CategoryId,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        var dtos = _mapper.Map<List<MarketplaceProductDto>>(items);

        return new PaginatedList<MarketplaceProductDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize
        );
    }
}