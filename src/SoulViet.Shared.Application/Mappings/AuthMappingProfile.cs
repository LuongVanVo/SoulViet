using AutoMapper;
using SoulViet.Shared.Application.Features.Auth.Commands.Register;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Application.Mappings;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<RegisterCommand, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());
    }
}