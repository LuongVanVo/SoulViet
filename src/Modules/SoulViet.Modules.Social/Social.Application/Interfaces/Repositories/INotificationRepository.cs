using SoulViet.Modules.Social.Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Notification>> GetByUserIdAsync(Guid userId, int limit = 20, CancellationToken cancellationToken = default);
        Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
        Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
        Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
