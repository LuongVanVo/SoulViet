using System.Text.Json.Serialization;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Queries.GetVouchersWithPagination;

public class GetVouchersWithPaginationQuery : IRequest<PaginatedList<VoucherDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }

    [JsonIgnore]
    public Guid? PartnerId { get; set; } // Id của đối tác để lọc voucher riêng của đối tác
    [JsonIgnore]
    public bool IsAdmin { get; set; } // Biến để xác định nếu người dùng là admin hay không
}