using AutoMapper;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Mappings;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<MasterOrder, MasterOrderDto>();
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();

        CreateMap<MasterOrder, OrderHistoryItemDto>()
            .ForMember(dest => dest.PaymentStatus,
                opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
            .ForMember(dest => dest.PaymentMethod,
                opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
            .ForMember(dest => dest.TotalShops,
                opt => opt.MapFrom(src => src.VendorOrders.Count))
            .ForMember(dest => dest.ShopNames,
                opt => opt.MapFrom(src => src.VendorOrders.Select(vo => vo.PartnerId.ToString()).ToList()))
            .ForMember(dest => dest.TotalAmount,
                opt => opt.MapFrom(src => src.GrandTotal))
            .ForMember(dest => dest.CreatedDate,
                opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<OrderItem, OrderItemDetailDto>()
            .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom(src => src.ProductNameSnapshot))
            .ForMember(dest => dest.ProductImage,
                opt => opt.MapFrom(src => src.ProductImageSnapshot));

        CreateMap<Order, VendorOrderDetailDto>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom(src => src.OrderItems));

        CreateMap<MasterOrder, MasterOrderDetailDto>()
            .ForMember(dest => dest.PaymentStatus,
                opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
            .ForMember(dest => dest.PaymentStatus,
                opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
            .ForMember(dest => dest.ReceiverName,
                opt => opt.MapFrom(src => src.VendorOrders.FirstOrDefault() != null ? src.VendorOrders.First().ReceiverName : null))
            .ForMember(dest => dest.ReceiverPhone,
                opt => opt.MapFrom(src => src.VendorOrders.FirstOrDefault() != null ? src.VendorOrders.First().ReceiverPhone : null))
            .ForMember(dest => dest.ShippingAddress,
                opt => opt.MapFrom(src => src.VendorOrders.FirstOrDefault() != null ? src.VendorOrders.First().ShippingAddress : null));
    }
}