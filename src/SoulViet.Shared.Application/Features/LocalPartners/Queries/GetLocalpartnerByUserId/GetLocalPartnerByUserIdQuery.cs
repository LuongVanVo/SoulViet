using MediatR;
using SoulViet.Shared.Application.DTOs;

namespace SoulViet.Shared.Application.Features.LocalPartners.Queries.GetLocalpartnerByUserId;

public class GetLocalPartnerByUserIdQuery : IRequest<LocalPartnerInfoDto>
{
    public Guid UserId { get; set; }
}