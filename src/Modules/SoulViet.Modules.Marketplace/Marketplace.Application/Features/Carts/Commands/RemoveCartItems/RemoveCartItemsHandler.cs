using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.RemoveCartItems
{
    public class RemoveCartItemsHandler : IRequestHandler<RemoveCartItemsCommand, bool>
    {
        private readonly ICartRepository _cartRepository;

        public RemoveCartItemsHandler(ICartRepository cartRepository) 
        {
            _cartRepository = cartRepository;
        }

        public async Task<bool> Handle(RemoveCartItemsCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (cart == null || !cart.Items.Any()) return true;

            var itemsToRemove = cart.Items.Where(i => request.CartItemIds.Contains(i.Id)).ToList();
            if (!itemsToRemove.Any()) return true;

            foreach (var item in itemsToRemove)
            {
                cart.Items.Remove(item);
            }

            await _cartRepository.SaveCartAsync(request.UserId, cart, cancellationToken);

            return true;
        }
    }
}