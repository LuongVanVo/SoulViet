using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Modules.Social.Social.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ICacheService _cacheService;

        public UserService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<Dictionary<Guid, UserMinimalDto>> GetUsersMinimalInfoAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
        {
            var result = new Dictionary<Guid, UserMinimalDto>();
            var distinctIds = userIds.Distinct().ToList();

            // Redis
            var fetchTasks = distinctIds.Select(async id =>
            {
                var cacheKey = $"user:info:{id}";
                var userDto = await _cacheService.GetAsync<UserMinimalDto>(cacheKey, cancellationToken);
                return new { Id = id, Dto = userDto };
            });

            var cacheResults = await Task.WhenAll(fetchTasks);

            foreach (var item in cacheResults)
            {
                if (item.Dto != null)
                {
                    result[item.Id] = item.Dto;
                }
                else
                {
                    // Fallback 
                    result[item.Id] = new UserMinimalDto
                    {
                        Id = item.Id,
                        FullName = "User",
                        AvatarUrl = null
                    };
                }
            }

            return result;
        }
    }
}