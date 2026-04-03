using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Commands.CreateCategory;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, MarketplaceCategoryResponse>
{
    private readonly IMarketplaceCategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    public CreateCategoryHandler(IMarketplaceCategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<MarketplaceCategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = _mapper.Map<MarketplaceCategory>(request);
        category.Id = Guid.NewGuid();
        category.Slug = GenerateSlug(request.Name);

        // Check if slug already exists
        bool isSlugExist = await _categoryRepository.IsSlugExistsAsync(category.Slug, cancellationToken);
        if (isSlugExist)
        {
            category.Slug = $"{category.Slug}-{DateTime.UtcNow.Ticks}";
        }

        await _categoryRepository.AddAsync(category, cancellationToken);

        return new MarketplaceCategoryResponse
        {
            Success = true,
            Message = "Category created successfully",
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl ?? string.Empty,
        };
    }

    // Helper method
    private string GenerateSlug(string phrase)
    {
        string str = RemoveAccent(phrase).ToLower();
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = Regex.Replace(str, @"\s+", "-").Trim();
        return str;
    }

    private string RemoveAccent(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}