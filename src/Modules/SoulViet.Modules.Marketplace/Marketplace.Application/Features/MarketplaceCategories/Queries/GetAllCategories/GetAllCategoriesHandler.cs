using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Queries.GetAllCategories;

public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, IEnumerable<MarketplaceCategoryDto>>
{
    private readonly IMarketplaceCategoryRepository _marketplaceCategoryRepository;
    private readonly IMapper _mapper;
    public GetAllCategoriesHandler(IMarketplaceCategoryRepository marketplaceCategoryRepository, IMapper mapper)
    {
        _marketplaceCategoryRepository = marketplaceCategoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MarketplaceCategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _marketplaceCategoryRepository.GetAllActiveAsync(cancellationToken);
        return _mapper.Map<IEnumerable<MarketplaceCategoryDto>>(categories);
    }
}