using Microsoft.AspNetCore.Http;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;

public interface IVnPayService
{
    string CreatePaymentUrl(MasterOrder masterOrder, HttpContext context);
}