using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Social.Social.Application.Interfaces.Repositories;
using SoulViet.Modules.Social.Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly SocialDbContext _context;

        public NotificationRepository(SocialDbContext context)
        {
            _context = context;
        }

        public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<List<Notification>> GetByUserIdAsync(Guid userId, int limit = 20, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .Where(n => n.RecipientUserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _context.Notifications.AddAsync(notification, cancellationToken);
        }

        public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            _context.Notifications.Update(notification);
        }

        public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var unread = await _context.Notifications
                .Where(n => n.RecipientUserId == userId && !n.IsRead)
                .ToListAsync(cancellationToken);

            foreach (var n in unread)
            {
                n.IsRead = true;
                n.ReadAt = DateTime.UtcNow;
            }
        }

        public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .CountAsync(n => n.RecipientUserId == userId && !n.IsRead, cancellationToken);
        }
    }
}
