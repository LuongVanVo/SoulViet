using SoulViet.Modules.Social.Social.Application.DTOs;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Social.Social.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ICacheService _cacheService;
        private readonly IUserRepository _userRepository;

        public UserService(ICacheService cacheService, IUserRepository userRepository)
        {
            _cacheService = cacheService;
            _userRepository = userRepository;
        }

        public async Task<Dictionary<Guid, UserMinimalDto>> GetUsersMinimalInfoAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
        {
            var result = new Dictionary<Guid, UserMinimalDto>();
            var distinctIds = userIds.Distinct().ToList();

            var fetchTasks = distinctIds.Select(async id =>
            {
                var cacheKey = $"user:info:{id}";
                var userDto = await _cacheService.GetAsync<UserMinimalDto>(cacheKey, cancellationToken);
                return new { Id = id, Dto = userDto };
            });

            var cacheResults = await Task.WhenAll(fetchTasks);

            var missingUserIds = new List<Guid>();

            foreach (var item in cacheResults)
            {
                if (item.Dto != null)
                {
                    result[item.Id] = item.Dto;
                }
                else
                {
                    missingUserIds.Add(item.Id);
                }
            }

            if (missingUserIds.Any())
            {
                var dbUsers = await _userRepository.GetUsersByIdsAsync(missingUserIds);
                foreach (var user in dbUsers)
                {
                    var userDto = new UserMinimalDto
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        AvatarUrl = user.AvatarUrl
                    };

                    result[user.Id] = userDto;

                    var cacheKey = $"user:info:{user.Id}";
                    await _cacheService.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(30), null, cancellationToken);
                }

                var stillMissing = missingUserIds.Except(dbUsers.Select(u => u.Id)).ToList();
                foreach (var id in stillMissing)
                {
                    result[id] = new UserMinimalDto
                    {
                        Id = id,
                        FullName = "User",
                        AvatarUrl = null
                    };
                }
            }

            return result;
        }
    }
}