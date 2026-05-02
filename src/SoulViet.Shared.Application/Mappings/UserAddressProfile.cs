using AutoMapper;
using SoulViet.Shared.Application.DTOs;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Application.Mappings;

public class UserAddressProfile : Profile
{
    public UserAddressProfile()
    {
        CreateMap<UserAddress, UserAddressDto>();
    }
}