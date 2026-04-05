using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetPublishedMarketplaceProducts;

public class GetPublishedMarketplaceProductsHandler : IRequestHandler<GetPublishedMarketplaceProductsQuery, PaginatedList<MarketplaceProductDto>>
{
    private readonly IMarketplaceProductRepository _marketplaceProductRepository;
    private readonly IMapper _mapper;
    public GetPublishedMarketplaceProductsHandler(IMarketplaceProductRepository marketplaceProductRepository, IMapper mapper)
    {
        _marketplaceProductRepository = marketplaceProductRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<MarketplaceProductDto>> Handle(GetPublishedMarketplaceProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _marketplaceProductRepository.GetPublishedProductsAsync(
            request.SearchTerm,
            request.CategoryId,
            request.MinPrice,
            request.MaxPrice,
            request.ProvinceId,
            request.SortBy,
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