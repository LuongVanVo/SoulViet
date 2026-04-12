using AutoMapper;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Vouchers.Commands.CreateVoucher;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Mappings;

public class VoucherProfile : Profile
{
    public VoucherProfile()
    {
        CreateMap<Voucher, VoucherDto>()
            .ForMember(dest => dest.PartnerName,
                opt => opt.Ignore());

        CreateMap<CreateVoucherCommand, Voucher>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StartDate,
                opt => opt.MapFrom(src => src.StartDate.ToUniversalTime()))
            .ForMember(dest => dest.EndDate,
                opt => opt.MapFrom(src => src.EndDate.ToUniversalTime()));
    }
}