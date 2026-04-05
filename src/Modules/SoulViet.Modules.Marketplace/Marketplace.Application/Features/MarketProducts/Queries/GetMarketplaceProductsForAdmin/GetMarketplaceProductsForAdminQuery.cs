using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.MarketProducts.Queries.GetMarketplaceProductsForAdmin;

public class GetMarketplaceProductsForAdminQuery : IRequest<PaginatedList<MarketplaceProductDto>>
{
    public string? SearchTerm { get; set; }
    public bool? IsVerified { get; set; }
    public bool? IsActive { get; set; }
    public Guid? CategoryId { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}