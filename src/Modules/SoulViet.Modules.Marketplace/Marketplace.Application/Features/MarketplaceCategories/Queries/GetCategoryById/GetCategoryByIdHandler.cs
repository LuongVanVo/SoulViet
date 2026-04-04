using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Queries.GetCategoryById;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, MarketplaceCategoryDto>
{
    private readonly IMarketplaceCategoryRepository _marketplaceCategoryRepository;
    private readonly IMapper _mapper;
    public GetCategoryByIdHandler(IMarketplaceCategoryRepository marketplaceCategoryRepository, IMapper mapper)
    {
        _marketplaceCategoryRepository = marketplaceCategoryRepository;
        _mapper = mapper;
    }

    public async Task<MarketplaceCategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _marketplaceCategoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null || !category.IsActive)
        {
            throw new NotFoundException("Category not found");
        }
        return _mapper.Map<MarketplaceCategoryDto>(category);
    }
}