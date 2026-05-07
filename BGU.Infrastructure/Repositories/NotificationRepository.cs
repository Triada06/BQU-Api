using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Infrastructure.Repositories;

public class NotificationRepository(AppDbContext db) : BaseRepository<Notification>(db), INotificationRepository
{
    private readonly AppDbContext _db = db;

    public async Task<bool> BulkMarkAllAsReadAsync(IEnumerable<string> notificationIds)
    {
        var res = await _db.Notifications.Where(x => notificationIds.Contains(x.Id))
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.IsRead, true));
        return res > 0;
    }
}