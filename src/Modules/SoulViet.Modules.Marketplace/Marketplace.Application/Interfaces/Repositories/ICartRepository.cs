using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Cart cart, CancellationToken cancellationToken = default);
    Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SaveCartAsync(Guid userId, Cart cart, CancellationToken cancellationToken = default);
    Task<List<CartItem>> GetItemsByIdsAsync(List<Guid> itemIds, Guid userId, CancellationToken cancellationToken = default);
}