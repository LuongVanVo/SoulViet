namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketplaceCategories.Results;

public class MarketplaceCategoryResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}