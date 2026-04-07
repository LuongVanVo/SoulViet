using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.RemoveCartItems
{
    public class RemoveCartItemsCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public List<Guid> CartItemIds { get; set; } = new();
    }
}