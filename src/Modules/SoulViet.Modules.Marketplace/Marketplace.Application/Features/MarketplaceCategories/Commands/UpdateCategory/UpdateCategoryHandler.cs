using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Commands.UpdateCategory;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, MarketplaceCategoryResponse>
{
    private readonly IMarketplaceCategoryRepository _marketplaceCategoryRepository;
    private readonly IMapper _mapper;
    public UpdateCategoryHandler(IMarketplaceCategoryRepository marketplaceCategoryRepository, IMapper mapper)
    {
        _marketplaceCategoryRepository = marketplaceCategoryRepository;
        _mapper = mapper;
    }

    public async Task<MarketplaceCategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            // Find the category by ID
            var category = await _marketplaceCategoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (category == null)
                throw new NotFoundException("Category not found");

            if (!category.IsActive)
                throw new BadRequestException("This category is inactive and cannot be updated");

            string oldName = category.Name;

            _mapper.Map(request, category);

            if (request.CategoryType.HasValue && request.CategoryType.Value != category.CategoryType)
            {
                category.CategoryType = request.CategoryType.Value;
            }

            if (oldName != category.Name)
            {
                category.Slug = GenerateSlug(category.Name);

                bool isSlugExists =
                    await _marketplaceCategoryRepository.IsSlugExistsAsync(category.Slug, cancellationToken);
                if (isSlugExists)
                    category.Slug = $"{category.Slug}-{DateTime.UtcNow.Ticks}";
            }

            // Update auditing fields
            category.LastModifiedAt = DateTime.UtcNow;
            category.LastModifiedBy = request.ModifiedBy.ToString();

            await _marketplaceCategoryRepository.UpdateAsync(category, cancellationToken);

            return new MarketplaceCategoryResponse
            {
                Success = true,
                Message = "Category updated successfully",
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
        str = str.Replace("đ", "d").Replace("Đ", "d");
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