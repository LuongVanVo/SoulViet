using SoulViet.Modules.Social.Social.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Interfaces.Repositories
{
    public interface IUserService
    {
        Task<Dictionary<Guid, UserMinimalDto>> GetUsersMinimalInfoAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);
    }
}
