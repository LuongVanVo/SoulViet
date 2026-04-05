using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetMarketplaceProductsForAdmin;

public class GetMarketplaceProductsForAdminHandler : IRequestHandler<GetMarketplaceProductsForAdminQuery, PaginatedList<MarketplaceProductDto>>
{
    private readonly IMarketplaceProductRepository _marketplaceProductRepository;
    private readonly IMapper _mapper;
    public GetMarketplaceProductsForAdminHandler(IMapper mapper, IMarketplaceProductRepository marketplaceProductRepository)
    {
        _mapper = mapper;
        _marketplaceProductRepository = marketplaceProductRepository;
    }

    public async Task<PaginatedList<MarketplaceProductDto>> Handle(GetMarketplaceProductsForAdminQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _marketplaceProductRepository.GetPagedForAdminAsync(
            request.SearchTerm,
            request.IsVerified,
            request.IsActive,
            request.CategoryId,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        var dtos = _mapper.Map<List<MarketplaceProductDto>>(items);

        return new PaginatedList<MarketplaceProductDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}