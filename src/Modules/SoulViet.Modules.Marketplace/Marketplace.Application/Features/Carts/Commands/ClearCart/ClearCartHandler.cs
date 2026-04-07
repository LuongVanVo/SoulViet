using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.ClearCart
{
    public class ClearCartHandler : IRequestHandler<ClearCartCommand, bool>
    {
        private readonly ICartRepository _cartRepository;
        
        public ClearCartHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<bool> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (cart != null && cart.Items.Any())
            {
                cart.Items.Clear(); 
                await _cartRepository.SaveCartAsync(request.UserId, cart, cancellationToken);
            }

            return true;
        }
    }
}