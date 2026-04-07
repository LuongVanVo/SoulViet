using SoulViet.Modules.Marketplace.Marketplace.Domain.Common;

namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

public class Cart : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    public CartItem? GetCartItem(Guid marketplaceProductId, string? metadata)
    {
        return Items.FirstOrDefault(x =>
            x.MarketplaceProductId == marketplaceProductId &&
            x.ItemMetadata == metadata);
    }

    public CartItem? GetCartItemById(Guid cartItemId)
    {
        return Items.FirstOrDefault(x => x.Id == cartItemId);
    }
}