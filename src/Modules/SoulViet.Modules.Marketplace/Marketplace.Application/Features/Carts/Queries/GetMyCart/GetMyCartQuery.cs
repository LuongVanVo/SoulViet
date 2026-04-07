using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Carts.Queries.GetMyCart;

public class GetMyCartQuery : IRequest<CartDto>
{
    public Guid UserId { get; set; }
}