using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Commands.ClearCart
{
    public class ClearCartCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
    }
}